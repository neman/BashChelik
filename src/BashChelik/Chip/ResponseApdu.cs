using System;

namespace BashChelik.Chip;

/// <summary>A chip's response: its data and the two-byte status word.</summary>
internal readonly struct ResponseApdu
{
    public ResponseApdu(byte[] data, ushort statusWord)
    {
        Data = data;
        StatusWord = statusWord;
    }

    public byte[] Data { get; }

    public ushort StatusWord { get; }

    public bool IsSuccess => StatusWord == 0x9000;

    /// <summary>Splits raw response bytes into data and status word.</summary>
    public static ResponseApdu FromBytes(byte[] raw, int length)
    {
        if (length < 2)
            throw new DocumentReadException(DocumentReadFailure.ChipError, "The chip sent a response without a status word.");
        var data = new byte[length - 2];
        Array.Copy(raw, data, data.Length);
        return new ResponseApdu(data, (ushort)((raw[length - 2] << 8) | raw[length - 1]));
    }
}
