using System;
using System.Collections.Generic;

namespace BashChelik;

/// <summary>Everything read from one document, with the chip bytes it was made from.</summary>
public sealed class IdentityDocument
{
    private static readonly IReadOnlyDictionary<ushort, byte[]> NoEvidence = new Dictionary<ushort, byte[]>();

    /// <summary>Creates a document from its parts.</summary>
    public IdentityDocument(DocumentKind kind, DocumentData documentData, PersonalData personalData, Address address,
        Portrait portrait, IReadOnlyList<HolderCertificate> holderCertificates)
    {
        Kind = kind;
        DocumentData = documentData;
        PersonalData = personalData;
        Address = address;
        Portrait = portrait;
        HolderCertificates = holderCertificates;
    }

    /// <summary>Which document this is.</summary>
    public DocumentKind Kind { get; }

    /// <summary>Facts about the document.</summary>
    public DocumentData DocumentData { get; }

    /// <summary>Facts about the holder.</summary>
    public PersonalData PersonalData { get; }

    /// <summary>The holder's registered residence.</summary>
    public Address Address { get; }

    /// <summary>The holder's photograph.</summary>
    public Portrait Portrait { get; }

    /// <summary>The holder's certificates.</summary>
    public IReadOnlyList<HolderCertificate> HolderCertificates { get; }

    /// <summary>The chip files this document was made from, by file id, as the chip stores them.</summary>
    internal IReadOnlyDictionary<ushort, byte[]> Evidence { get; init; } = NoEvidence;

    /// <summary>Whether the document is valid on <paramref name="day"/>: issued by then and not yet expired.</summary>
    public bool IsValidOn(DateTime day) =>
        !DocumentData.IsExpiredOn(day) && !(DocumentData.IssueDate is DateTime issued && issued.Date > day.Date);
}
