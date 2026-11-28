using Xunit;

namespace BashChelik.Tests.Documents;

public class BirthDateTests
{
    [Fact]
    public void A_year_alone_keeps_the_year_and_is_no_date()
    {
        var date = new BirthDate(1975, null, null);

        Assert.Equal(1975, date.Year);
        Assert.Null(date.AsDateTime);
    }

    [Fact]
    public void A_day_that_does_not_exist_is_no_date()
    {
        Assert.Null(new BirthDate(2001, 2, 29).AsDateTime);
    }
}
