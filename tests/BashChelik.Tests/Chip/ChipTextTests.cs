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

    [Fact]
    public void A_placeholder_birth_date_is_null()
    {
        Assert.Null(ChipText.BirthDate("01010001"));
    }
}
