namespace BashChelik.Chip;

/// <summary>File <c>0F04</c>: the address.</summary>
internal static class AddressLayout
{
    public const ushort Fid = 0x0F04;

    private const ushort State = 1568;
    private const ushort Community = 1569;
    private const ushort Place = 1570;
    private const ushort Street = 1571;
    private const ushort HouseNumber = 1572;
    private const ushort HouseLetter = 1573;
    private const ushort Entrance = 1574;
    private const ushort Floor = 1575;
    private const ushort ApartmentNumber = 1578;
    private const ushort AddressDate = 1580;
    private const ushort Label = 1581;

    public static Address Read(TagRecords r) => new(
        state: r.Text(State),
        community: r.Text(Community),
        place: r.Text(Place),
        street: r.Text(Street),
        houseNumber: r.Text(HouseNumber),
        houseLetter: r.Text(HouseLetter),
        entrance: r.Text(Entrance),
        floor: r.Text(Floor),
        apartmentNumber: r.Text(ApartmentNumber),
        addressDate: ChipText.Date(r.Text(AddressDate)),
        label: r.Text(Label));
}
