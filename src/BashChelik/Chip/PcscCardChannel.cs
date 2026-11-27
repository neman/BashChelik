using System;
using PCSC;
using PCSC.Exceptions;

namespace BashChelik.Chip;

/// <summary>A card in a PC/SC reader, connected in shared mode.</summary>
internal sealed class PcscCardChannel : ICardChannel
{
    private readonly ISCardContext _context;
    private readonly ICardReader _reader;
    private readonly string _readerName;

    private PcscCardChannel(ISCardContext context, ICardReader reader, string readerName, byte[] atr)
    {
        _context = context;
        _reader = reader;
        _readerName = readerName;
        Atr = atr;
    }

    public byte[] Atr { get; }

    /// <summary>The names of the connected readers; empty when there are none.</summary>
    public static string[] ListReaders()
    {
        try
        {
            using var context = ContextFactory.Instance.Establish(SCardScope.System);
            return context.GetReaders() ?? Array.Empty<string>();
        }
        catch (Exception e) when (e is NoReadersAvailableException or NoServiceException)
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>Connects to the card in <paramref name="readerName"/>.</summary>
    public static PcscCardChannel Connect(string readerName)
    {
        ISCardContext? context = null;
        ICardReader? reader = null;
        try
        {
            context = ContextFactory.Instance.Establish(SCardScope.System);
            reader = context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any);
            return new PcscCardChannel(context, reader, readerName, reader.GetAttrib(SCardAttribute.AtrString));
        }
        catch (Exception e) when (e is PCSCException)
        {
            reader?.Dispose();
            context?.Dispose();
            throw Translate(e, readerName, connected: false);
        }
    }

    public ResponseApdu Transmit(byte[] command)
    {
        var buffer = new byte[258];
        try
        {
            var length = _reader.Transmit(command, buffer);
            return ResponseApdu.FromBytes(buffer, length);
        }
        catch (Exception e) when (e is PCSCException)
        {
            throw Translate(e, _readerName, connected: true);
        }
    }

    public IDisposable BeginTransaction()
    {
        try
        {
            return new QuietTransaction(_reader.Transaction(SCardReaderDisposition.Leave));
        }
        catch (PCSCException e) when (e.SCardError == SCardError.ResetCard)
        {
            // Another application reset the shared card; reconnect once. Every read selects its applet again.
            try
            {
                _reader.Reconnect(SCardShareMode.Shared, SCardProtocol.Any, SCardReaderDisposition.Leave);
                return new QuietTransaction(_reader.Transaction(SCardReaderDisposition.Leave));
            }
            catch (Exception retry) when (retry is PCSCException)
            {
                throw Translate(retry, _readerName, connected: true);
            }
        }
        catch (Exception e) when (e is PCSCException)
        {
            throw Translate(e, _readerName, connected: true);
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
        _context.Dispose();
    }

    /// <summary>
    /// Turns a PC/SC error into the reader's own. <paramref name="connected"/> tells whether the card was
    /// already in use: a missing card then means it was removed, and a missing reader that it was unplugged.
    /// </summary>
    internal static Exception Translate(Exception e, string readerName, bool connected) => e switch
    {
        NoServiceException or NoReadersAvailableException or UnknownReaderException when !connected =>
            new DocumentReadException(DocumentReadFailure.NoReader, $"No card reader named \"{readerName}\" is connected.", innerException: e),
        ReaderUnavailableException when !connected =>
            new DocumentReadException(DocumentReadFailure.NoReader, $"The card reader \"{readerName}\" is not available.", innerException: e),
        NoServiceException or NoReadersAvailableException or UnknownReaderException or ReaderUnavailableException =>
            new DocumentReadException(DocumentReadFailure.NoReader, $"The card reader \"{readerName}\" was disconnected.", innerException: e),
        NoSmartcardException or UnpoweredCardException when !connected =>
            new DocumentReadException(DocumentReadFailure.NoCard, $"There is no card in \"{readerName}\".", innerException: e),
        NoSmartcardException or UnpoweredCardException or RemovedCardException =>
            new DocumentReadException(DocumentReadFailure.CardRemoved, $"The card was removed from \"{readerName}\".", innerException: e),
        PCSCException pcsc =>
            new DocumentReadException(DocumentReadFailure.ChipError, $"The card reader reported {pcsc.SCardError}.", innerException: e),
        _ => e,
    };
}
