using System;

namespace BashChelik.Chip;

/// <summary>The ISO 7816-4 commands the reader sends.</summary>
internal static class Apdu
{
    /// <summary><c>00 A4 04 00 Lc AID</c>: select an applet by its AID.</summary>
    public static byte[] SelectAid(byte[] aid)
    {
        var command = new byte[5 + aid.Length];
        command[1] = 0xA4;
        command[2] = 0x04;
        command[4] = (byte)aid.Length;
        Array.Copy(aid, 0, command, 5, aid.Length);
        return command;
    }

    /// <summary><c>00 A4 08 00 02 FID</c>: select a file by its path from the applet.</summary>
    public static byte[] SelectFile(ushort fid) =>
        new byte[] { 0x00, 0xA4, 0x08, 0x00, 0x02, (byte)(fid >> 8), (byte)fid };

    /// <summary><c>00 B0 hi lo Le</c>: read <paramref name="length"/> bytes of the selected file from <paramref name="offset"/>.</summary>
    public static byte[] ReadBinary(int offset, byte length) =>
        new byte[] { 0x00, 0xB0, (byte)(offset >> 8), (byte)offset, length };

    /// <summary><c>00 C0 00 00 Le</c>: fetch the data a T=0 chip announced with <c>61xx</c>.</summary>
    public static byte[] GetResponse(byte length) =>
        new byte[] { 0x00, 0xC0, 0x00, 0x00, length };
}
