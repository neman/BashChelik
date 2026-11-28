using System.IO;
using System.Threading;

namespace BashChelik.Chip;

/// <summary>Sends commands over a channel and handles the T=0 replies <c>61xx</c> and <c>6Cxx</c>.</summary>
internal sealed class CardCommands(ICardChannel channel)
{
    public ICardChannel Channel { get; } = channel;

    /// <summary>Sends a command and returns the final response, whatever its status word.</summary>
    public ResponseApdu Send(byte[] command, CancellationToken ct)
    {
        var response = TransmitWithLengthCorrection(command, ct);
        if (response.StatusWord >> 8 != 0x61)
            return response;

        using var data = new MemoryStream();
        while (response.StatusWord >> 8 == 0x61)
        {
            data.Write(response.Data, 0, response.Data.Length);
            response = TransmitWithLengthCorrection(Apdu.GetResponse((byte)response.StatusWord), ct);
        }

        data.Write(response.Data, 0, response.Data.Length);
        return new ResponseApdu(data.ToArray(), response.StatusWord);
    }

    /// <summary>Sends a command and returns its data, or throws <see cref="DocumentReadFailure.ChipError"/>.</summary>
    public byte[] SendExpectingSuccess(byte[] command, CancellationToken ct)
    {
        var response = Send(command, ct);
        if (!response.IsSuccess)
            throw new DocumentReadException(
                DocumentReadFailure.ChipError,
                $"The chip answered {response.StatusWord:X4}.",
                response.StatusWord);
        return response.Data;
    }

    private ResponseApdu TransmitWithLengthCorrection(byte[] command, CancellationToken ct)
    {
        var response = Transmit(command, ct);
        // Only the five-byte READ BINARY and GET RESPONSE commands carry Le.
        if (response.StatusWord >> 8 != 0x6C || command.Length != 5)
            return response;

        var corrected = (byte[])command.Clone();
        corrected[4] = (byte)response.StatusWord;
        return Transmit(corrected, ct);
    }

    private ResponseApdu Transmit(byte[] command, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Channel.Transmit(command);
    }
}
