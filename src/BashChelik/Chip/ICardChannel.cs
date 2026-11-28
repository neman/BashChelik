using System;

namespace BashChelik.Chip;

/// <summary>The only way the library reaches a chip: send one command, get its response.</summary>
internal interface ICardChannel : IDisposable
{
    /// <summary>The chip's answer to reset.</summary>
    byte[] Atr { get; }

    /// <summary>Sends one command and returns the chip's response.</summary>
    ResponseApdu Transmit(byte[] command);

    /// <summary>Keeps other applications off the card until the returned object is disposed.</summary>
    IDisposable BeginTransaction();
}
