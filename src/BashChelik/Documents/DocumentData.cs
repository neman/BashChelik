using System;

namespace BashChelik;

/// <summary>Facts about the document itself.</summary>
public sealed class DocumentData
{
    /// <summary>Creates document data; any value may be <c>null</c> when the document lacks it.</summary>
    public DocumentData(string? number, string? typeCode, string? serialNumber, string? chipSerialNumber,
        DateTime? issueDate, DateTime? expiryDate, string? issuingAuthority, string? documentName)
    {
        Number = number;
        TypeCode = typeCode;
        SerialNumber = serialNumber;
        ChipSerialNumber = chipSerialNumber;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        IssuingAuthority = issuingAuthority;
        DocumentName = documentName;
    }

    /// <summary>The document's registration number.</summary>
    public string? Number { get; }

    /// <summary>The type code the document carries, such as <c>ID</c>, <c>IF</c> or <c>RP</c>.</summary>
    public string? TypeCode { get; }

    /// <summary>The document's serial number.</summary>
    public string? SerialNumber { get; }

    /// <summary>The chip's serial number.</summary>
    public string? ChipSerialNumber { get; }

    /// <summary>The day the document was issued.</summary>
    public DateTime? IssueDate { get; }

    /// <summary>The last day the document is valid.</summary>
    public DateTime? ExpiryDate { get; }

    /// <summary>The authority that issued the document.</summary>
    public string? IssuingAuthority { get; }

    /// <summary>The document's name as printed on it.</summary>
    public string? DocumentName { get; }

    /// <summary>Whether <paramref name="day"/> is after the expiry date; a document without one never expires.</summary>
    public bool IsExpiredOn(DateTime day) => ExpiryDate is DateTime expiry && day.Date > expiry.Date;
}
