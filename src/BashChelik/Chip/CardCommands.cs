using System.Threading;

namespace BashChelik.Chip;

/// <summary>Sends commands over a channel and handles the T=0 replies <c>61xx</c> and <c>6Cxx</c>.</summary>
internal sealed class CardCommands(ICardChannel channel)
{
    public ICardChannel Channel { get; } = channel;

    /// <summary>Sends a command and returns the final response, whatever its status word.</summary>
    public ResponseApdu Send(byte[] command, CancellationToken ct)
    {
        var response = Transmit(command, ct);

        if (response.StatusWord >> 8 == 0x6C)
        {
            var resent = (byte[])command.Clone();
            resent[resent.Length - 1] = (byte)response.StatusWord;
            response = Transmit(resent, ct);
        }

        while (response.StatusWord >> 8 == 0x61)
            response = Transmit(Apdu.GetResponse((byte)response.StatusWord), ct);

        return response;
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

    private ResponseApdu Transmit(byte[] command, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Channel.Transmit(command);
    }
}
