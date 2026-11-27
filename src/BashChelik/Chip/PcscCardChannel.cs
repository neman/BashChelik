using System;
using PCSC;
using PCSC.Exceptions;

namespace BashChelik.Chip;

/// <summary>A card in a PC/SC reader, connected in shared mode.</summary>
internal sealed class PcscCardChannel : ICardChannel
{
    private readonly ISCardContext _context;
    private readonly ICardReader _reader;

    private PcscCardChannel(ISCardContext context, ICardReader reader, byte[] atr)
    {
        _context = context;
        _reader = reader;
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
        var context = ContextFactory.Instance.Establish(SCardScope.System);
        try
        {
            var reader = context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any);
            return new PcscCardChannel(context, reader, reader.GetAttrib(SCardAttribute.AtrString));
        }
        catch (Exception e)
        {
            context.Dispose();
            throw Translate(e, readerName);
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
            throw Translate(e, null);
        }
    }

    public IDisposable BeginTransaction()
    {
        try
        {
            return ((CardReader)_reader).Transaction(SCardReaderDisposition.Leave);
        }
        catch (Exception e) when (e is PCSCException)
        {
            throw Translate(e, null);
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
        _context.Dispose();
    }

    private static Exception Translate(Exception e, string? readerName) => e switch
    {
        DocumentReadException => e,
        UnknownReaderException or ReaderUnavailableException or NoReadersAvailableException =>
            new DocumentReadException(DocumentReadFailure.NoReader, $"No card reader named \"{readerName}\" is connected.", innerException: e),
        NoSmartcardException or UnpoweredCardException =>
            new DocumentReadException(DocumentReadFailure.NoCard, "There is no card in the reader.", innerException: e),
        RemovedCardException =>
            new DocumentReadException(DocumentReadFailure.CardRemoved, "The card was removed from the reader.", innerException: e),
        PCSCException pcsc =>
            new DocumentReadException(DocumentReadFailure.ChipError, $"The card reader reported {pcsc.SCardError}.", innerException: e),
        _ => e,
    };
}
