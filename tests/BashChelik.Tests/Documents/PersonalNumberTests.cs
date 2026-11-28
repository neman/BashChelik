using System;
using Xunit;

namespace BashChelik.Tests.Documents;

public class PersonalNumberTests
{
    [Fact]
    public void A_valid_number_tells_birth_date_and_male_sex()
    {
        var number = new PersonalNumber("0101990710008");

        Assert.True(number.HasValidChecksum);
        Assert.Equal(new DateTime(1990, 1, 1), number.BirthDate!.AsDateTime);
        Assert.Equal(Sex.Male, number.Sex);
    }

    [Fact]
    public void A_valid_number_from_this_century_tells_female_sex()
    {
        var number = new PersonalNumber("1505005715006");

        Assert.True(number.HasValidChecksum);
        Assert.Equal(new DateTime(2005, 5, 15), number.BirthDate!.AsDateTime);
        Assert.Equal(Sex.Female, number.Sex);
    }

    [Fact]
    public void A_wrong_check_digit_tells_nothing()
    {
        var number = new PersonalNumber("0101990710001");

        Assert.False(number.HasValidChecksum);
        Assert.Null(number.BirthDate);
        Assert.Null(number.Sex);
    }

    [Fact]
    public void Any_text_is_kept_as_the_chip_holds_it()
    {
        var number = new PersonalNumber("12345");

        Assert.False(number.HasValidChecksum);
        Assert.Equal("12345", number.Value);
    }
}
