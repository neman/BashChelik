using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace BashChelik.Tests.Simulator;

/// <summary>Holder certificates made up in the test run, stored the way the CardEdge applet stores them.</summary>
internal static class InventedCertificates
{
    public const string CardEdgeAid = "A000000063504B43532D3135";

    public static X509Certificate2 Authentication { get; } = Create("CN=Test Authentication",
        X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature);

    public static X509Certificate2 Signing { get; } = Create("CN=Test Signing",
        X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature);

    /// <summary>An applet as on the probed card: two files that are not certificates, then the two certificates.</summary>
    public static Dictionary<ushort, byte[]> CardEdgeFiles() => new()
    {
        [0x7100] = new byte[] { 0xFC, 0x00, 0x04, 0x71, 0x00, 0x00 },
        [0x7101] = new byte[] { 0xAC, 0x00, 0x38, 0x00, 0x00, 0x00 },
        [0x7102] = Compressed(Authentication.RawData),
        [0x7103] = Plain(Signing.RawData),
    };

    /// <summary><c>length (2, LE) | 01 00 | der length (2, LE) | zlib(der)</c>.</summary>
    public static byte[] Compressed(byte[] der)
    {
        var zlib = Zlib(der);
        var body = new byte[4 + zlib.Length];
        body[0] = 0x01;
        body[2] = (byte)der.Length;
        body[3] = (byte)(der.Length >> 8);
        Array.Copy(zlib, 0, body, 4, zlib.Length);
        return WithLength(body);
    }

    /// <summary><c>length (2, LE) | der</c>.</summary>
    public static byte[] Plain(byte[] der) => WithLength(der);

    private static byte[] WithLength(byte[] body)
    {
        var file = new byte[2 + body.Length];
        file[0] = (byte)body.Length;
        file[1] = (byte)(body.Length >> 8);
        Array.Copy(body, 0, file, 2, body.Length);
        return file;
    }

    private static byte[] Zlib(byte[] data)
    {
        using var output = new MemoryStream();
        output.WriteByte(0x78);
        output.WriteByte(0x9C);
        using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
            deflate.Write(data, 0, data.Length);
        uint a = 1, b = 0;
        foreach (var x in data)
        {
            a = (a + x) % 65521;
            b = (b + a) % 65521;
        }

        var adler = (b << 16) | a;
        output.Write(new[] { (byte)(adler >> 24), (byte)(adler >> 16), (byte)(adler >> 8), (byte)adler }, 0, 4);
        return output.ToArray();
    }

    private static X509Certificate2 Create(string subject, X509KeyUsageFlags usage)
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var request = new CertificateRequest(subject, key, HashAlgorithmName.SHA256);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(usage, critical: true));
        return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1));
    }
}
