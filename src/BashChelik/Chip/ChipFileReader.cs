using System;
using System.IO;
using System.Threading;

namespace BashChelik.Chip;

/// <summary>Selects applets and reads whole files, header first, then the content in chunks.</summary>
internal sealed class ChipFileReader(CardCommands commands)
{
    private const int Chunk = 255;
    private const int HeaderLength = 4;

    public CardCommands Commands { get; } = commands;

    public void SelectApplet(byte[] aid, CancellationToken ct) =>
        Commands.SendExpectingSuccess(Apdu.SelectAid(aid), ct);

    public bool TrySelectApplet(byte[] aid, CancellationToken ct) =>
        Commands.Send(Apdu.SelectAid(aid), ct).IsSuccess;

    public ChipFile ReadFile(ushort fid, CancellationToken ct)
    {
        Commands.SendExpectingSuccess(Apdu.SelectFile(fid), ct);
        var header = Commands.SendExpectingSuccess(Apdu.ReadBinary(0, HeaderLength), ct);
        if (header.Length < HeaderLength)
            throw new DocumentReadException(DocumentReadFailure.ChipError, $"File {fid:X4} has no header.");

        int total = HeaderLength + (header[2] | (header[3] << 8));
        using var raw = new MemoryStream(total);
        raw.Write(header, 0, HeaderLength);
        while (raw.Length < total)
        {
            var length = (byte)Math.Min(Chunk, total - raw.Length);
            var chunk = Commands.SendExpectingSuccess(Apdu.ReadBinary((int)raw.Length, length), ct);
            if (chunk.Length == 0)
                throw new DocumentReadException(DocumentReadFailure.ChipError, $"File {fid:X4} ended before its declared length.");
            raw.Write(chunk, 0, chunk.Length);
        }

        return new ChipFile(fid, raw.ToArray());
    }
}
