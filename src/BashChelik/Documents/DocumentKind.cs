namespace BashChelik;

/// <summary>Which supported document a chip carries, and what follows from it.</summary>
public sealed class DocumentKind
{
    private DocumentKind(string name, bool hasForeignerData, bool addressHasState)
    {
        Name = name;
        HasForeignerData = hasForeignerData;
        AddressHasState = addressHasState;
    }

    /// <summary>The identity document of a Serbian citizen (lična karta).</summary>
    public static DocumentKind IdentityCard { get; } = new("Identity card", hasForeignerData: false, addressHasState: true);

    /// <summary>The identity document of a foreigner with residence in Serbia (lična karta za strance).</summary>
    public static DocumentKind ForeignerIdentityCard { get; } = new("Foreigner identity card", hasForeignerData: true, addressHasState: true);

    /// <summary>The residence permit of a foreigner (boravišna dozvola).</summary>
    public static DocumentKind ResidencePermit { get; } = new("Residence permit", hasForeignerData: true, addressHasState: false);

    /// <summary>The kind's name in English.</summary>
    public string Name { get; }

    /// <summary>Whether personal data holds a foreigner's nationality, status, purpose of stay and note.</summary>
    public bool HasForeignerData { get; }

    /// <summary>Whether the address names a state.</summary>
    public bool AddressHasState { get; }

    /// <inheritdoc />
    public override string ToString() => Name;
}
