using System;

namespace BashChelik.Chip;

/// <summary>
/// A file as the chip stores it: a 4-byte header (tag and length, both little-endian) and the content.
/// <see cref="Raw"/> keeps the exact bytes for verification.
/// </summary>
internal sealed class ChipFile
{
    public ChipFile(ushort fid, byte[] raw)
    {
        if (raw.Length < 4)
            throw new DocumentReadException(DocumentReadFailure.ChipError, $"File {fid:X4} is shorter than its header.");
        Fid = fid;
        Raw = raw;
    }

    public ushort Fid { get; }

    public byte[] Raw { get; }

    public ushort HeaderTag => (ushort)(Raw[0] | (Raw[1] << 8));

    public int DeclaredLength => Raw[2] | (Raw[3] << 8);

    public byte[] Content
    {
        get
        {
            var content = new byte[Math.Min(DeclaredLength, Raw.Length - 4)];
            Array.Copy(Raw, 4, content, 0, content.Length);
            return content;
        }
    }
}
