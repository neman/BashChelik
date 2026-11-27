using System;

namespace BashChelik;

/// <summary>A date of birth that may lack its day or month, keeping the year the document carries.</summary>
public sealed class BirthDate
{
    /// <summary>Creates a birth date; <paramref name="month"/> and <paramref name="day"/> are <c>null</c> when unknown.</summary>
    public BirthDate(int year, int? month, int? day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    /// <summary>The year of birth.</summary>
    public int Year { get; }

    /// <summary>The month of birth, when known.</summary>
    public int? Month { get; }

    /// <summary>The day of birth, when known.</summary>
    public int? Day { get; }

    /// <summary>The full date, or <c>null</c> when the day or month is unknown or the date does not exist.</summary>
    public DateTime? AsDateTime =>
        Month is int month && Day is int day
            && Year is >= 1 and <= 9999 && month is >= 1 and <= 12
            && day >= 1 && day <= DateTime.DaysInMonth(Year, month)
                ? new DateTime(Year, month, day)
                : null;
}
