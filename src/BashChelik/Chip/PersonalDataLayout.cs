namespace BashChelik.Chip;

/// <summary>File <c>0F03</c>: personal data.</summary>
internal static class PersonalDataLayout
{
    public const ushort Fid = 0x0F03;

    private const ushort PersonalNumber = 1558;
    private const ushort Surname = 1559;
    private const ushort GivenName = 1560;
    private const ushort ParentGivenName = 1561;
    private const ushort Sex = 1562;
    private const ushort PlaceOfBirth = 1563;
    private const ushort CommunityOfBirth = 1564;
    private const ushort StateOfBirth = 1565;
    private const ushort DateOfBirth = 1566;
    private const ushort StateOfBirthCode = 1567;
    private const ushort StatusOfForeigner = 1582;
    private const ushort Nationality = 1583;
    private const ushort PurposeOfStay = 1683;
    private const ushort Note = 1684;

    public static PersonalData Read(TagRecords r, DocumentKind kind) => new(
        personalNumber: r.Text(PersonalNumber) is { } number ? new BashChelik.PersonalNumber(number) : null,
        surname: r.Text(Surname),
        givenName: r.Text(GivenName),
        parentGivenName: r.Text(ParentGivenName),
        sex: r.Text(Sex),
        birthDate: ChipText.BirthDate(r.Text(DateOfBirth)),
        placeOfBirth: r.Text(PlaceOfBirth),
        communityOfBirth: r.Text(CommunityOfBirth),
        stateOfBirth: r.Text(StateOfBirth),
        stateOfBirthCode: r.Text(StateOfBirthCode),
        foreignerData: kind.HasForeignerData
            ? new ForeignerData(r.Text(Nationality), r.Text(StatusOfForeigner), r.Text(PurposeOfStay), r.Text(Note))
            : null);
}
