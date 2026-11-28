namespace BashChelik;

/// <summary>Why a document could not be read.</summary>
public enum DocumentReadFailure
{
    /// <summary>No card reader with the given name is connected.</summary>
    NoReader,

    /// <summary>The reader has no card in it.</summary>
    NoCard,

    /// <summary>The card was removed while it was being read.</summary>
    CardRemoved,

    /// <summary>The card holds a document the library does not support.</summary>
    UnsupportedDocument,

    /// <summary>The chip answered a command with an error.</summary>
    ChipError,
}
