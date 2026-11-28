using System;
using BashChelik.Chip;
using Xunit;

namespace BashChelik.Tests.Chip;

public class ChipTextTests
{
    [Fact]
    public void A_full_date_is_read_day_month_year()
    {
        Assert.Equal(new DateTime(2030, 5, 15), ChipText.Date("15052030"));
    }

    [Theory]
    [InlineData("01010001")]
    [InlineData("1505203")]
    [InlineData("31022030")]
    [InlineData("ab052030")]
    [InlineData(null)]
    public void A_placeholder_or_malformed_date_is_null(string? text)
    {
        Assert.Null(ChipText.Date(text));
    }

    [Fact]
    public void A_birth_date_without_day_and_month_keeps_the_year()
    {
        var date = ChipText.BirthDate("00001975")!;

        Assert.Equal(1975, date.Year);
        Assert.Null(date.Month);
        Assert.Null(date.Day);
    }

    [Theory]
    [InlineData("01010001")]
    [InlineData("01010000")]
    [InlineData("45132000")]
    [InlineData("31042000")]
    [InlineData("29022001")]
    public void A_placeholder_or_invalid_birth_date_is_null(string text)
    {
        Assert.Null(ChipText.BirthDate(text));
    }

    [Fact]
    public void A_birth_date_can_have_a_known_month_or_a_leap_day()
    {
        var partial = ChipText.BirthDate("00022000")!;
        Assert.Equal(2, partial.Month);
        Assert.Null(partial.Day);
        Assert.Equal(new DateTime(2000, 2, 29), ChipText.BirthDate("29022000")!.AsDateTime);
    }
}
