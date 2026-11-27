using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using BashChelik.Chip;

namespace BashChelik.ConsoleApp;

/// <summary>
/// Prints the structure of the chip and no value from it: which applets answer, which files exist,
/// their header tags and lengths, which record tags are present, the shape of the signature files,
/// and where the certificates say their revocation is published. Nothing is written to disk.
/// </summary>
internal static class Probe
{
    private static readonly (string Name, string Aid)[] DocumentApplets =
    {
        ("identity card", "F381000002534552494401"),
        ("foreigner identity card", "F381000002534552494601"),
        ("residence permit", "F381000002534552525001"),
        ("service identity card", "F381000002534552454C01"),
    };

    private const string CardEdgeAid = "A000000063504B43532D3135";

    private static readonly ushort[] Files =
        { 0x0F02, 0x0F03, 0x0F04, 0x0F06, 0x0F1C, 0x0F1D, 0x0F1B, 0x0D10, 0x0FA1, 0x0F12, 0x0FA2, 0x0F07 };

    private static readonly ushort[] RecordFiles = { 0x0F02, 0x0F03, 0x0F04 };
    private static readonly ushort[] SignatureFiles = { 0x0F1C, 0x0F1D };

    public static int Run(string readerName)
    {
        var ct = CancellationToken.None;
        using var channel = PcscCardChannel.Connect(readerName);
        var files = new ChipFileReader(new CardCommands(channel));

        Console.WriteLine($"Reader: {readerName}");
        Console.WriteLine($"ATR: {Hex(channel.Atr)}");

        Console.WriteLine("\nDocument applets:");
        string? documentAid = null;
        foreach (var (name, aid) in DocumentApplets)
        {
            var answers = files.TrySelectApplet(Parse(aid), ct);
            Console.WriteLine($"  {name,-25} {aid}  {(answers ? "answers" : "no")}");
            if (answers && documentAid is null && name != "service identity card")
                documentAid = aid;
        }

        if (documentAid is not null)
            ProbeDocumentFiles(files, documentAid, ct);

        ProbeCardEdge(files, ct);
        return 0;
    }

    private static void ProbeDocumentFiles(ChipFileReader files, string aid, CancellationToken ct)
    {
        Console.WriteLine("\nFiles (FID: SELECT status, header tag, declared length):");
        foreach (var fid in Files)
        {
            files.SelectApplet(Parse(aid), ct);
            var select = files.Commands.Send(Apdu.SelectFile(fid), ct);
            if (!select.IsSuccess)
            {
                Console.WriteLine($"  {fid:X4}: {select.StatusWord:X4}");
                continue;
            }

            var header = files.Commands.Send(Apdu.ReadBinary(0, 4), ct);
            if (!header.IsSuccess || header.Data.Length < 4)
            {
                Console.WriteLine($"  {fid:X4}: 9000, header read {header.StatusWord:X4}");
                continue;
            }

            int tag = header.Data[0] | (header.Data[1] << 8), length = header.Data[2] | (header.Data[3] << 8);
            Console.WriteLine($"  {fid:X4}: 9000, tag {tag}, length {length}");
        }

        foreach (var fid in RecordFiles)
        {
            files.SelectApplet(Parse(aid), ct);
            var content = files.ReadFile(fid, ct).Content;
            Console.WriteLine($"\nRecord tags in {fid:X4} (present, no values): {string.Join(", ", RecordTags(content))}");
        }

        foreach (var fid in SignatureFiles)
        {
            files.SelectApplet(Parse(aid), ct);
            Console.WriteLine($"\nSignature file {fid:X4}:");
            DescribeSignature(files.ReadFile(fid, ct));
        }
    }

    private static IEnumerable<int> RecordTags(byte[] content)
    {
        for (int i = 0; i + 4 <= content.Length;)
        {
            int tag = content[i] | (content[i + 1] << 8), length = content[i + 2] | (content[i + 3] << 8);
            yield return tag;
            i += 4 + length;
        }
    }

    private static void DescribeSignature(ChipFile file)
    {
        foreach (var skip in new[] { 4, 8, 0 })
        {
            var cms = new SignedCms();
            try
            {
                cms.Decode(file.Raw.Skip(skip).ToArray());
            }
            catch (Exception)
            {
                continue;
            }

            Console.WriteLine($"  CMS starts at offset {skip}");
            Console.WriteLine($"  content type: {cms.ContentInfo.ContentType.Value}");
            foreach (var signer in cms.SignerInfos)
            {
                Console.WriteLine($"  digest: {signer.DigestAlgorithm.FriendlyName ?? signer.DigestAlgorithm.Value}");
                Console.WriteLine($"  signed attributes: {string.Join(", ", signer.SignedAttributes.Cast<CryptographicAttributeObject>().Select(a => a.Oid.Value))}");
                foreach (var attribute in signer.SignedAttributes)
                    if (attribute.Oid.Value == "1.2.840.113549.1.9.5")
                        Console.WriteLine($"  signing time present: yes");
                if (signer.Certificate is { } certificate)
                {
                    Console.WriteLine($"  signer: {certificate.Subject}");
                    Console.WriteLine($"  issuer: {certificate.Issuer}");
                    Console.WriteLine($"  signer valid: {certificate.NotBefore:yyyy-MM-dd} to {certificate.NotAfter:yyyy-MM-dd}");
                    PrintRevocation(certificate);
                }
            }

            Console.WriteLine("  signed content (structure; digests shown by length only):");
            PrintAsn1(new AsnReader(cms.ContentInfo.Content, AsnEncodingRules.BER), 2);
            return;
        }

        Console.WriteLine($"  no CMS found; header tag {file.HeaderTag}, length {file.DeclaredLength}, first bytes after header {Hex(file.Raw.Skip(4).Take(8).ToArray())}");
    }

    private static void PrintAsn1(AsnReader reader, int depth)
    {
        var indent = new string(' ', depth * 2);
        while (reader.HasData)
        {
            var tag = reader.PeekTag();
            if (tag.IsConstructed)
            {
                Console.WriteLine($"{indent}{Name(tag)} {{");
                var inner = tag.TagClass == TagClass.Universal && tag.TagValue == 17
                    ? reader.ReadSetOf(tag)
                    : reader.ReadSequence(tag);
                PrintAsn1(inner, depth + 1);
                Console.WriteLine($"{indent}}}");
            }
            else if (tag.HasSameClassAndValue(Asn1Tag.Integer))
            {
                Console.WriteLine($"{indent}INTEGER {reader.ReadInteger()}");
            }
            else if (tag.HasSameClassAndValue(Asn1Tag.ObjectIdentifier))
            {
                Console.WriteLine($"{indent}OID {reader.ReadObjectIdentifier()}");
            }
            else
            {
                var value = reader.ReadEncodedValue();
                Console.WriteLine($"{indent}{Name(tag)} ({value.Length} bytes encoded)");
            }
        }
    }

    private static string Name(Asn1Tag tag) =>
        tag.TagClass == TagClass.Universal ? ((UniversalTagNumber)tag.TagValue).ToString() : $"[{tag.TagClass} {tag.TagValue}]";

    private static void ProbeCardEdge(ChipFileReader files, CancellationToken ct)
    {
        Console.WriteLine("\nCardEdge applet:");
        if (!files.TrySelectApplet(Parse(CardEdgeAid), ct))
        {
            Console.WriteLine("  does not answer");
            return;
        }

        foreach (var fid in Enumerable.Range(0x7000, 0x200).Select(f => (ushort)f))
        {
            var select = files.Commands.Send(Apdu.SelectFile(fid), ct);
            if (!select.IsSuccess)
                continue;

            var start = files.Commands.Send(Apdu.ReadBinary(0, 4), ct);
            Console.WriteLine($"  {fid:X4}: first bytes {(start.IsSuccess ? Hex(start.Data) : start.StatusWord.ToString("X4"))}");
            if (fid >= 0x7100 && start.IsSuccess && TryReadCertificate(files, start.Data, ct) is { } certificate)
            {
                Console.WriteLine($"    certificate, issuer {certificate.Issuer}, valid {certificate.NotBefore:yyyy-MM-dd} to {certificate.NotAfter:yyyy-MM-dd}");
                Console.WriteLine($"    key usage: {certificate.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault()?.KeyUsages}");
                PrintRevocation(certificate);
            }

            files.SelectApplet(Parse(CardEdgeAid), ct);
        }
    }

    private static X509Certificate2? TryReadCertificate(ChipFileReader files, byte[] start, CancellationToken ct)
    {
        foreach (var length in new[] { start[0] | (start[1] << 8), (start[0] << 8) | start[1] })
        {
            if (length is < 100 or > 8000)
                continue;
            var bytes = ReadPlain(files, 2 + length, ct).Skip(2).ToArray();
            try
            {
                var der = bytes[0] == 0x01 && bytes[1] == 0x00 ? Inflate(bytes.Skip(4).ToArray()) : bytes;
                return X509CertificateLoader.LoadCertificate(der);
            }
            catch (Exception)
            {
            }
        }

        return null;
    }

    private static byte[] ReadPlain(ChipFileReader files, int total, CancellationToken ct)
    {
        using var raw = new MemoryStream();
        while (raw.Length < total)
        {
            var chunk = files.Commands.SendExpectingSuccess(Apdu.ReadBinary((int)raw.Length, (byte)Math.Min(255, total - raw.Length)), ct);
            if (chunk.Length == 0)
                break;
            raw.Write(chunk);
        }

        return raw.ToArray();
    }

    private static byte[] Inflate(byte[] zlib)
    {
        using var output = new MemoryStream();
        using (var inflater = new ZLibStream(new MemoryStream(zlib), CompressionMode.Decompress))
            inflater.CopyTo(output);
        return output.ToArray();
    }

    private static void PrintRevocation(X509Certificate2 certificate)
    {
        foreach (var extension in certificate.Extensions)
        {
            if (extension.Oid?.Value is "2.5.29.31" or "1.3.6.1.5.5.7.1.1")
            {
                var name = extension.Oid.Value == "2.5.29.31" ? "CRL" : "authority info";
                foreach (var line in extension.Format(true).Split('\n').Where(l => l.Contains("URL=") || l.Contains("http")))
                    Console.WriteLine($"    {name}: {line.Trim()}");
            }
        }
    }

    private static byte[] Parse(string hex) =>
        Enumerable.Range(0, hex.Length / 2).Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16)).ToArray();

    private static string Hex(byte[] bytes) => string.Concat(bytes.Select(b => b.ToString("X2")));
}
