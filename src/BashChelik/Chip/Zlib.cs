using System.IO;
using System.IO.Compression;

namespace BashChelik.Chip;

/// <summary>zlib (RFC 1950) inflation, the same on every target: skip the 2-byte header, inflate, ignore the checksum.</summary>
internal static class Zlib
{
    public static byte[] Inflate(byte[] zlib)
    {
        using var output = new MemoryStream();
        using (var inflater = new DeflateStream(new MemoryStream(zlib, 2, zlib.Length - 2), CompressionMode.Decompress))
            inflater.CopyTo(output);
        return output.ToArray();
    }
}
