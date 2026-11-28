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
            Guarded("document files", () => ProbeDocumentFiles(files, documentAid, ct));

        Guarded("CardEdge applet", () => ProbeCardEdge(files, ct));
        return 0;
    }

    private static void Guarded(string part, Action probe)
    {
        try
        {
            probe();
        }
        catch (Exception e) when (e is not DocumentReadException)
        {
            Console.WriteLine($"  probe of {part} stopped: {e.GetType().Name}: {e.Message}");
        }
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
            if (TryReadProbeFile(files, aid, fid, ct) is { } file)
                Console.WriteLine($"\nRecord tags in {fid:X4} (present, no values): {string.Join(", ", RecordTags(file.Content))}");
        }

        var covered = new List<(ushort Fid, string Variant, byte[] Bytes)>();
        foreach (var fid in Files.Except(SignatureFiles))
        {
            files.SelectApplet(Parse(aid), ct);
            if (!files.Commands.Send(Apdu.SelectFile(fid), ct).IsSuccess)
                continue;
            if (TryReadProbeFile(files, aid, fid, ct) is { } file)
                covered.Add((fid, "header + declared length", file.Raw));

            try
            {
                files.SelectApplet(Parse(aid), ct);
                covered.Add((fid, "whole file", ReadAllocated(files, fid, ct)));
            }
            catch (DocumentReadException e) when (e.Reason == DocumentReadFailure.ChipError)
            {
                Console.WriteLine($"  {fid:X4}: reading whole file stopped: {e.Message}");
            }
        }

        foreach (var fid in SignatureFiles)
        {
            Console.WriteLine($"\nSignature file {fid:X4}:");
            if (TryReadProbeFile(files, aid, fid, ct) is { } file)
                DescribeSignature(file, covered);
        }
    }

    private static ChipFile? TryReadProbeFile(ChipFileReader files, string aid, ushort fid, CancellationToken ct)
    {
        try
        {
            files.SelectApplet(Parse(aid), ct);
            return files.ReadFile(fid, ct);
        }
        catch (DocumentReadException e) when (e.Reason == DocumentReadFailure.ChipError)
        {
            Console.WriteLine($"  {fid:X4}: reading header + declared length stopped: {e.Message}");
            return null;
        }
    }

    private static byte[] ReadAllocated(ChipFileReader files, ushort fid, CancellationToken ct)
    {
        files.Commands.SendExpectingSuccess(Apdu.SelectFile(fid), ct);
        using var raw = new MemoryStream();
        int le = 255;
        ushort last = 0x9000;
        while (raw.Length < 0x7F00)
        {
            var chunk = files.Commands.Send(Apdu.ReadBinary((int)raw.Length, (byte)le), ct);
            last = chunk.StatusWord;
            if (chunk.Data.Length > 0 && (chunk.IsSuccess || chunk.StatusWord == 0x6282))
            {
                raw.Write(chunk.Data);
                if (!chunk.IsSuccess)
                    break;
                continue;
            }

            if (le == 1)
                break;
            le = Math.Max(1, le / 2);
        }

        Console.WriteLine($"  {fid:X4}: whole file read as {raw.Length} bytes, last status {last:X4}");
        return raw.ToArray();
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

    private static void DescribeSignature(ChipFile file, List<(ushort Fid, string Variant, byte[] Bytes)> covered)
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

            DescribeDigests(cms.ContentInfo.Content, covered);
            return;
        }

        Console.WriteLine($"  no CMS found; header tag {file.HeaderTag}, length {file.DeclaredLength}, first bytes after header {Hex(file.Raw.Skip(4).Take(8).ToArray())}");
    }

    /// <summary>
    /// Finds where each file's SHA-256 sits in the signed content, without printing any digest.
    /// What no file explains is printed as offset and length; only short gaps (format bytes) as hex.
    /// </summary>
    private static void DescribeDigests(byte[] content, List<(ushort Fid, string Variant, byte[] Bytes)> covered)
    {
        Console.WriteLine($"  signed content: {content.Length} bytes");
        var explained = new bool[content.Length];
        using var sha = SHA256.Create();
        foreach (var (fid, variant, bytes) in covered)
        {
            var digest = sha.ComputeHash(bytes);
            var at = IndexOf(content, digest);
            Console.WriteLine($"    {fid:X4} ({variant}, {bytes.Length} bytes): {(at < 0 ? "not found" : $"digest at offset {at}")}");
            if (at >= 0)
                for (int i = at; i < at + digest.Length; i++)
                    explained[i] = true;
        }

        for (int slot = 0; slot + 32 <= content.Length; slot += 32)
        {
            if (explained[slot])
                continue;
            var wanted = content.AsSpan(slot, 32).ToArray();
            foreach (var (fid, variant, bytes) in covered.Where(c => c.Variant == "whole file"))
                for (int length = 4; length <= bytes.Length; length++)
                    if (SHA256.HashData(bytes.AsSpan(0, length)).AsSpan().SequenceEqual(wanted))
                    {
                        Console.WriteLine($"    slot at {slot}: the first {length} bytes of {fid:X4}");
                        for (int i = slot; i < slot + 32; i++)
                            explained[i] = true;
                    }
        }

        for (int i = 0; i < content.Length;)
        {
            if (explained[i])
            {
                i++;
                continue;
            }

            int start = i;
            while (i < content.Length && !explained[i])
                i++;
            var gap = content.Skip(start).Take(i - start).ToArray();
            Console.WriteLine($"    unexplained at {start}, {gap.Length} bytes{(gap.Length <= 8 ? ": " + Hex(gap) : "")}");
        }
    }

    private static int IndexOf(byte[] haystack, byte[] needle)
    {
        for (int i = 0; i + needle.Length <= haystack.Length; i++)
            if (haystack.AsSpan(i, needle.Length).SequenceEqual(needle))
                return i;
        return -1;
    }

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
            byte[] bytes;
            try
            {
                bytes = ReadPlain(files, 2 + length, ct).Skip(2).ToArray();
            }
            catch (DocumentReadException e)
            {
                Console.WriteLine($"    reading {2 + length} bytes stopped: {e.StatusWord:X4}");
                continue;
            }

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
