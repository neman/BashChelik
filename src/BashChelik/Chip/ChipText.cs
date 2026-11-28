using System;

namespace BashChelik.Chip;

/// <summary>Dates as the chip writes them: eight digits, <c>DDMMYYYY</c>.</summary>
internal static class ChipText
{
    private const int PlaceholderYear = 1;

    /// <summary>The date, or <c>null</c> for a placeholder (year 0001) or anything that is not a real date.</summary>
    public static DateTime? Date(string? ddmmyyyy) =>
        Parts(ddmmyyyy) is var (day, month, year) && year != PlaceholderYear
            ? new BirthDate(year, month, day).AsDateTime
            : null;

    /// <summary>A birth date, where <c>00</c> marks an unknown day or month; <c>null</c> for a placeholder or malformed text.</summary>
    public static BirthDate? BirthDate(string? ddmmyyyy)
    {
        if (Parts(ddmmyyyy) is not var (day, month, year) || year <= PlaceholderYear || month > 12 || day > 31)
            return null;
        if (month != 0 && day > DateTime.DaysInMonth(year, month))
            return null;

        int? knownMonth = month == 0 ? null : month;
        int? knownDay = day == 0 || knownMonth is null ? null : day;
        return new BirthDate(year, knownMonth, knownDay);
    }

    private static (int Day, int Month, int Year)? Parts(string? text)
    {
        if (text is null || text.Length != 8)
            return null;
        foreach (var c in text)
            if (c is < '0' or > '9')
                return null;
        return (int.Parse(text.Substring(0, 2)), int.Parse(text.Substring(2, 2)), int.Parse(text.Substring(4, 4)));
    }
}
