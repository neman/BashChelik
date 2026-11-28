using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace BashChelik.Chip;

/// <summary>
/// The CardEdge applet, which holds the holder certificates. Each is a file of
/// <c>length (2, LE)</c> followed by <c>01 00 | der length (2, LE) | zlib(der)</c> or by the DER itself.
/// The probed card keeps them at <c>7102</c> and <c>7103</c>; files <c>7100</c> to <c>710F</c> are tried.
/// </summary>
internal static class CardEdge
{
    public static readonly byte[] Aid = { 0xA0, 0x00, 0x00, 0x00, 0x63, 0x50, 0x4B, 0x43, 0x53, 0x2D, 0x31, 0x35 };

    private const ushort FirstFile = 0x7100;
    private const ushort LastFile = 0x710F;
    private const byte DerSequence = 0x30;
    private const int LengthPrefixSize = 2;

    public static IReadOnlyList<HolderCertificate> ReadCertificates(ChipFileReader files, CancellationToken ct)
    {
        var certificates = new List<HolderCertificate>();
        if (!files.TrySelectApplet(Aid, ct))
            return certificates;

        for (var fid = FirstFile; fid <= LastFile; fid++)
        {
            if (!files.Commands.Send(Apdu.SelectFile(fid), ct).IsSuccess)
                continue;
            var start = files.Commands.Send(Apdu.ReadBinary(0, 4), ct);
            if (!start.IsSuccess || start.Data.Length < 4 || !LooksLikeCertificate(start.Data))
                continue;

            int length = start.Data[0] | (start.Data[1] << 8);
            if (length < 4 || LengthPrefixSize + length > Apdu.MaxBinaryFileLength)
                continue;

            var body = ReadBody(files.Commands, length, ct);
            if (body is not null && DecodeCertificate(body) is { } certificate)
                certificates.Add(certificate);
        }

        return certificates;
    }

    private static HolderCertificate? DecodeCertificate(byte[] body)
    {
        try
        {
            var der = body[0] == 0x01 && body[1] == 0x00 ? Zlib.Inflate(body.Skip(4).ToArray()) : body;
            if (der.Length == 0)
                return null;
            var certificate = Load(der);
            return new HolderCertificate(PurposeOf(certificate), certificate);
        }
        catch (Exception e) when (e is InvalidDataException or CryptographicException or ArgumentException)
        {
            return null;
        }
    }

    private static bool LooksLikeCertificate(byte[] start) =>
        (start[2] == 0x01 && start[3] == 0x00) || start[2] == DerSequence;

    private static CertificatePurpose PurposeOf(X509Certificate2 certificate) =>
        certificate.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault() is { } usage
            && usage.KeyUsages.HasFlag(X509KeyUsageFlags.NonRepudiation)
            ? CertificatePurpose.Signing
            : CertificatePurpose.Authentication;

    private static byte[]? ReadBody(CardCommands commands, int length, CancellationToken ct)
    {
        using var bytes = new MemoryStream(length);
        while (bytes.Length < length)
        {
            var remaining = length - (int)bytes.Length;
            var response = commands.Send(
                Apdu.ReadBinary(LengthPrefixSize + (int)bytes.Length, (byte)Math.Min(255, remaining)), ct);
            if (!response.IsSuccess || response.Data.Length == 0 || response.Data.Length > remaining)
                return null;
            bytes.Write(response.Data, 0, response.Data.Length);
        }

        return bytes.ToArray();
    }

    private static X509Certificate2 Load(byte[] der) =>
#if NET9_0_OR_GREATER
        X509CertificateLoader.LoadCertificate(der);
#else
        new(der);
#endif
}
