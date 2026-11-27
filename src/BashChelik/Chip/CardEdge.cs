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
            certificates.Add(ReadCertificate(files.Commands, fid, length, ct));
        }

        return certificates;
    }

    private static HolderCertificate ReadCertificate(CardCommands commands, ushort fid, int length, CancellationToken ct)
    {
        try
        {
            if (length < 4)
                throw new InvalidDataException("too short");
            var body = ReadExactly(commands, 2, length, ct);
            var der = body[0] == 0x01 && body[1] == 0x00 ? Zlib.Inflate(body.Skip(4).ToArray()) : body;
            if (der.Length == 0)
                throw new InvalidDataException("empty");
            var certificate = Load(der);
            return new HolderCertificate(PurposeOf(certificate), certificate);
        }
        catch (Exception e) when (e is InvalidDataException or CryptographicException or ArgumentException or IndexOutOfRangeException)
        {
            throw new DocumentReadException(DocumentReadFailure.ChipError, $"File {fid:X4} in the certificate applet is not a readable certificate.", innerException: e);
        }
    }

    private static bool LooksLikeCertificate(byte[] start) =>
        (start[2] == 0x01 && start[3] == 0x00) || start[2] == DerSequence;

    private static CertificatePurpose PurposeOf(X509Certificate2 certificate) =>
        certificate.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault() is { } usage
            && usage.KeyUsages.HasFlag(X509KeyUsageFlags.NonRepudiation)
            ? CertificatePurpose.Signing
            : CertificatePurpose.Authentication;

    private static byte[] ReadExactly(CardCommands commands, int offset, int length, CancellationToken ct)
    {
        using var bytes = new MemoryStream(length);
        while (bytes.Length < length)
        {
            var chunk = commands.SendExpectingSuccess(
                Apdu.ReadBinary(offset + (int)bytes.Length, (byte)Math.Min(255, length - bytes.Length)), ct);
            if (chunk.Length == 0)
                throw new DocumentReadException(DocumentReadFailure.ChipError, "A certificate file ended before its declared length.");
            bytes.Write(chunk, 0, chunk.Length);
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
