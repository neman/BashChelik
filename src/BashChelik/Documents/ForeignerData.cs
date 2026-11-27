namespace BashChelik;

/// <summary>Personal data that only a foreigner's document carries.</summary>
public sealed class ForeignerData
{
    /// <summary>Creates foreigner data.</summary>
    public ForeignerData(string? nationality, string? status, string? purposeOfStay, string? note)
    {
        Nationality = nationality;
        Status = status;
        PurposeOfStay = purposeOfStay;
        Note = note;
    }

    /// <summary>The holder's nationality.</summary>
    public string? Nationality { get; }

    /// <summary>The holder's status as a foreigner.</summary>
    public string? Status { get; }

    /// <summary>The purpose of the holder's stay.</summary>
    public string? PurposeOfStay { get; }

    /// <summary>A note the document carries.</summary>
    public string? Note { get; }
}
