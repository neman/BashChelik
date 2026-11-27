using System;
using System.Linq;

namespace BashChelik;

/// <summary>
/// The holder's personal number (JMBG), exactly as the document carries it.
/// When its check digit is right it also tells the date of birth and the sex.
/// </summary>
public sealed class PersonalNumber : IEquatable<PersonalNumber>
{
    private static readonly int[] Weights = { 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

    /// <summary>Creates a personal number from any text; nothing is rejected.</summary>
    public PersonalNumber(string value)
    {
        Value = value;
        HasValidChecksum = value.Length == 13
            && value.All(c => c is >= '0' and <= '9')
            && CheckDigit(value) == value[12] - '0';
    }

    /// <summary>The number as the document carries it.</summary>
    public string Value { get; }

    /// <summary>Whether the last digit is the check digit of the other twelve.</summary>
    public bool HasValidChecksum { get; }

    /// <summary>The date of birth encoded in the first seven digits, when the checksum is valid.</summary>
    public BirthDate? BirthDate
    {
        get
        {
            if (!HasValidChecksum)
                return null;
            int day = Digits(0, 2), month = Digits(2, 2), year = Digits(4, 3);
            return new BirthDate(year < 800 ? 2000 + year : 1000 + year, month, day);
        }
    }

    /// <summary>The sex encoded in digits ten to twelve, when the checksum is valid.</summary>
    public Sex? Sex => HasValidChecksum ? (Digits(9, 3) < 500 ? BashChelik.Sex.Male : BashChelik.Sex.Female) : null;

    /// <inheritdoc />
    public bool Equals(PersonalNumber? other) => other is not null && other.Value == Value;

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as PersonalNumber);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Value;

    private int Digits(int start, int count) => int.Parse(Value.Substring(start, count));

    private static int CheckDigit(string value)
    {
        int sum = Weights.Select((weight, i) => weight * (value[i] - '0')).Sum();
        int m = 11 - sum % 11;
        return m > 9 ? 0 : m;
    }
}
