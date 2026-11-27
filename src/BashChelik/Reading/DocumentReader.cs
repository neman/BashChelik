using System;
using System.Collections.Generic;
using System.Threading;
using BashChelik.Chip;

namespace BashChelik;

/// <summary>
/// A document in a card reader. Open it, read what you need, dispose it.
/// The card is shared between reads, and each read keeps other applications off it while it runs.
/// </summary>
public sealed class DocumentReader : IDisposable
{
    private readonly ICardChannel _channel;
    private readonly ChipFileReader _files;
    private readonly byte[] _applet;

    private DocumentReader(ICardChannel channel, DocumentKind kind)
    {
        _channel = channel;
        _files = new ChipFileReader(new CardCommands(channel));
        Kind = kind;
        _applet = DocumentRecognizer.AppletOf(kind);
    }

    /// <summary>Which document is in the reader.</summary>
    public DocumentKind Kind { get; }

    /// <summary>The names of the connected card readers; empty when there are none.</summary>
    public static string[] ListReaders() => PcscCardChannel.ListReaders();

    /// <summary>Opens the document in the reader named <paramref name="readerName"/>.</summary>
    /// <exception cref="DocumentReadException">No such reader, no card, or a card the library does not support.</exception>
    public static DocumentReader Open(string readerName, CancellationToken ct = default) =>
        Open(PcscCardChannel.Connect(readerName), ct);

    /// <summary>Opens the document in <paramref name="readerName"/>, reads all of it and closes it.</summary>
    public static IdentityDocument ReadAll(string readerName, CancellationToken ct = default)
    {
        using var reader = Open(readerName, ct);
        return reader.ReadDocument(ct);
    }

    internal static DocumentReader Open(ICardChannel channel, CancellationToken ct)
    {
        try
        {
            using (channel.BeginTransaction())
                return new DocumentReader(channel, DocumentRecognizer.Recognize(new ChipFileReader(new CardCommands(channel)), ct));
        }
        catch
        {
            channel.Dispose();
            throw;
        }
    }

    /// <summary>Reads the document data.</summary>
    public DocumentData ReadDocumentData(CancellationToken ct = default) =>
        InTransaction(ct, () => DocumentDataLayout.Read(Records(DocumentDataLayout.Fid, ct)));

    /// <summary>Reads the holder's personal data.</summary>
    public PersonalData ReadPersonalData(CancellationToken ct = default) =>
        InTransaction(ct, () => PersonalDataLayout.Read(Records(PersonalDataLayout.Fid, ct), Kind));

    /// <summary>Reads the holder's address.</summary>
    public Address ReadAddress(CancellationToken ct = default) =>
        InTransaction(ct, () => AddressLayout.Read(Records(AddressLayout.Fid, ct)));

    /// <summary>Reads the holder's portrait.</summary>
    public Portrait ReadPortrait(CancellationToken ct = default) =>
        InTransaction(ct, () => PortraitLayout.Read(_files.ReadFile(PortraitLayout.Fid, ct)));

    /// <summary>Reads the holder's certificates; no PIN is needed.</summary>
    public IReadOnlyList<HolderCertificate> ReadCertificates(CancellationToken ct = default) =>
        InTransaction(ct, () => CardEdge.ReadCertificates(_files, ct));

    /// <summary>Reads everything in one transaction, keeping the chip files for verification.</summary>
    public IdentityDocument ReadDocument(CancellationToken ct = default) => InTransaction(ct, () =>
    {
        var documentFile = _files.ReadFile(DocumentDataLayout.Fid, ct);
        var personalFile = _files.ReadFile(PersonalDataLayout.Fid, ct);
        var addressFile = _files.ReadFile(AddressLayout.Fid, ct);
        var portraitFile = _files.ReadFile(PortraitLayout.Fid, ct);
        var certificates = CardEdge.ReadCertificates(_files, ct);

        return new IdentityDocument(
            Kind,
            DocumentDataLayout.Read(TagRecords.Parse(documentFile.Content)),
            PersonalDataLayout.Read(TagRecords.Parse(personalFile.Content), Kind),
            AddressLayout.Read(TagRecords.Parse(addressFile.Content)),
            PortraitLayout.Read(portraitFile),
            certificates)
        {
            Evidence = new Dictionary<ushort, byte[]>
            {
                [documentFile.Fid] = documentFile.Raw,
                [personalFile.Fid] = personalFile.Raw,
                [addressFile.Fid] = addressFile.Raw,
                [portraitFile.Fid] = portraitFile.Raw,
            },
        };
    });

    /// <summary>Releases the card and the reader.</summary>
    public void Dispose() => _channel.Dispose();

    private TagRecords Records(ushort fid, CancellationToken ct) => TagRecords.Parse(_files.ReadFile(fid, ct).Content);

    private T InTransaction<T>(CancellationToken ct, Func<T> read)
    {
        using (_channel.BeginTransaction())
        {
            _files.SelectApplet(_applet, ct);
            return read();
        }
    }
}
