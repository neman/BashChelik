using System;

namespace BashChelik;

/// <summary>A document could not be read; <see cref="Reason"/> says why.</summary>
public sealed class DocumentReadException : Exception
{
    /// <summary>Creates the exception.</summary>
    public DocumentReadException(DocumentReadFailure reason, string message, ushort? statusWord = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Reason = reason;
        StatusWord = statusWord;
    }

    /// <summary>Why the document could not be read.</summary>
    public DocumentReadFailure Reason { get; }

    /// <summary>The chip's status word, when the chip refused a command.</summary>
    public ushort? StatusWord { get; }
}
