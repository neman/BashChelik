namespace BashChelik.Chip;

/// <summary>File <c>0F02</c>: document data.</summary>
internal static class DocumentDataLayout
{
    public const ushort Fid = 0x0F02;

    private const ushort Number = 1546;
    private const ushort TypeCode = 1547;
    private const ushort SerialNumber = 1548;
    private const ushort IssueDate = 1549;
    private const ushort ExpiryDate = 1550;
    private const ushort IssuingAuthority = 1551;
    private const ushort ChipSerialNumber = 1681;
    private const ushort DocumentName = 1682;

    public static DocumentData Read(TagRecords r) => new(
        number: r.Text(Number),
        typeCode: r.Text(TypeCode),
        serialNumber: r.Text(SerialNumber),
        chipSerialNumber: r.Text(ChipSerialNumber),
        issueDate: ChipText.Date(r.Text(IssueDate)),
        expiryDate: ChipText.Date(r.Text(ExpiryDate)),
        issuingAuthority: r.Text(IssuingAuthority),
        documentName: r.Text(DocumentName));
}
