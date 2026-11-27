namespace BashChelik;

/// <summary>Facts about the holder, fixed when the document was issued.</summary>
public sealed class PersonalData
{
    /// <summary>Creates personal data; any value may be <c>null</c> when the document lacks it.</summary>
    public PersonalData(PersonalNumber? personalNumber, string? surname, string? givenName, string? parentGivenName,
        string? sex, BirthDate? birthDate, string? placeOfBirth, string? communityOfBirth, string? stateOfBirth,
        string? stateOfBirthCode, ForeignerData? foreignerData)
    {
        PersonalNumber = personalNumber;
        Surname = surname;
        GivenName = givenName;
        ParentGivenName = parentGivenName;
        Sex = sex;
        BirthDate = birthDate;
        PlaceOfBirth = placeOfBirth;
        CommunityOfBirth = communityOfBirth;
        StateOfBirth = stateOfBirth;
        StateOfBirthCode = stateOfBirthCode;
        ForeignerData = foreignerData;
    }

    /// <summary>The personal number (JMBG).</summary>
    public PersonalNumber? PersonalNumber { get; }

    /// <summary>The surname.</summary>
    public string? Surname { get; }

    /// <summary>The given name.</summary>
    public string? GivenName { get; }

    /// <summary>A parent's given name.</summary>
    public string? ParentGivenName { get; }

    /// <summary>The sex as the document prints it.</summary>
    public string? Sex { get; }

    /// <summary>The date of birth.</summary>
    public BirthDate? BirthDate { get; }

    /// <summary>The place of birth.</summary>
    public string? PlaceOfBirth { get; }

    /// <summary>The community (municipality) of birth.</summary>
    public string? CommunityOfBirth { get; }

    /// <summary>The state of birth.</summary>
    public string? StateOfBirth { get; }

    /// <summary>The state of birth as a three-letter code.</summary>
    public string? StateOfBirthCode { get; }

    /// <summary>The foreigner's data; <c>null</c> on an identity card.</summary>
    public ForeignerData? ForeignerData { get; }
}
