using System;
using System.IO;
using System.Text;

namespace BashChelik.Tests.Simulator;

/// <summary>Builds chip files the way the chip stores them: 4-byte header, then tag-length-value records.</summary>
internal static class ChipFileBuilder
{
    public static byte[] File(ushort headerTag, params (ushort Tag, string Value)[] records)
    {
        using var content = new MemoryStream();
        foreach (var (tag, value) in records)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            content.Write(new[] { (byte)tag, (byte)(tag >> 8), (byte)bytes.Length, (byte)(bytes.Length >> 8) }, 0, 4);
            content.Write(bytes, 0, bytes.Length);
        }

        return Raw(headerTag, content.ToArray());
    }

    public static byte[] Raw(ushort headerTag, byte[] content)
    {
        var file = new byte[4 + content.Length];
        file[0] = (byte)headerTag;
        file[1] = (byte)(headerTag >> 8);
        file[2] = (byte)content.Length;
        file[3] = (byte)(content.Length >> 8);
        Array.Copy(content, 0, file, 4, content.Length);
        return file;
    }
}
