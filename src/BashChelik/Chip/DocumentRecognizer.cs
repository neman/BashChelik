using System.Linq;
using System.Threading;

namespace BashChelik.Chip;

/// <summary>Tells which document a chip carries, leaving its applet selected, or refuses the chip.</summary>
internal static class DocumentRecognizer
{
    private static readonly byte[] Atr2008 = Hex("3BB918008131FE9E8073FF614083000000DF");
    private static readonly byte[] ServiceIdentityCard = Hex("F381000002534552454C01");

    private static readonly (DocumentKind Kind, byte[] Aid)[] Supported =
    {
        (DocumentKind.IdentityCard, Hex("F381000002534552494401")),
        (DocumentKind.ForeignerIdentityCard, Hex("F381000002534552494601")),
        (DocumentKind.ResidencePermit, Hex("F381000002534552525001")),
    };

    /// <summary>The applet of each supported kind.</summary>
    public static byte[] AppletOf(DocumentKind kind) => Supported.First(s => s.Kind == kind).Aid;

    public static DocumentKind Recognize(ChipFileReader files, CancellationToken ct)
    {
        if (files.Commands.Channel.Atr.SequenceEqual(Atr2008))
            throw Unsupported("2008 identity card");

        foreach (var (kind, aid) in Supported)
            if (files.TrySelectApplet(aid, ct))
                return kind;

        if (files.TrySelectApplet(ServiceIdentityCard, ct))
            throw Unsupported("service identity card");
        throw Unsupported("unknown document");
    }

    private static DocumentReadException Unsupported(string name) =>
        new(DocumentReadFailure.UnsupportedDocument, $"The card holds a {name}, which is not supported.");

    private static byte[] Hex(string hex) =>
        Enumerable.Range(0, hex.Length / 2).Select(i => System.Convert.ToByte(hex.Substring(i * 2, 2), 16)).ToArray();
}
