using System;

namespace BashChelik;

/// <summary>The holder's registered residence (prebivalište).</summary>
public sealed class Address
{
    /// <summary>Creates an address; any value may be <c>null</c> when the document lacks it.</summary>
    public Address(string? state, string? community, string? place, string? street, string? houseNumber,
        string? houseLetter, string? entrance, string? floor, string? apartmentNumber, DateTime? addressDate, string? label)
    {
        State = state;
        Community = community;
        Place = place;
        Street = street;
        HouseNumber = houseNumber;
        HouseLetter = houseLetter;
        Entrance = entrance;
        Floor = floor;
        ApartmentNumber = apartmentNumber;
        AddressDate = addressDate;
        Label = label;
    }

    /// <summary>The state; <c>null</c> on a residence permit.</summary>
    public string? State { get; }

    /// <summary>The community (municipality).</summary>
    public string? Community { get; }

    /// <summary>The place.</summary>
    public string? Place { get; }

    /// <summary>The street.</summary>
    public string? Street { get; }

    /// <summary>The house number.</summary>
    public string? HouseNumber { get; }

    /// <summary>The letter after the house number.</summary>
    public string? HouseLetter { get; }

    /// <summary>The entrance.</summary>
    public string? Entrance { get; }

    /// <summary>The floor.</summary>
    public string? Floor { get; }

    /// <summary>The apartment number.</summary>
    public string? ApartmentNumber { get; }

    /// <summary>The day the address was registered.</summary>
    public DateTime? AddressDate { get; }

    /// <summary>The address label the document carries.</summary>
    public string? Label { get; }
}
