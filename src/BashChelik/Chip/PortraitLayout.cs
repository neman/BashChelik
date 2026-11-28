using System;

namespace BashChelik.Chip;

/// <summary>File <c>0F06</c>: a 4-byte inner header, then the portrait as JPEG.</summary>
internal static class PortraitLayout
{
    public const ushort Fid = 0x0F06;

    private const int InnerHeaderLength = 4;

    public static Portrait Read(ChipFile file)
    {
        var content = file.Content;
        if (content.Length < InnerHeaderLength + 2 || content[InnerHeaderLength] != 0xFF || content[InnerHeaderLength + 1] != 0xD8)
            throw new DocumentReadException(DocumentReadFailure.ChipError, "The portrait on the chip is not a JPEG.");

        var jpeg = new byte[content.Length - InnerHeaderLength];
        Array.Copy(content, InnerHeaderLength, jpeg, 0, jpeg.Length);
        return new Portrait(jpeg);
    }
}
