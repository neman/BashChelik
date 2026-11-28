using System;
using Xunit;

namespace BashChelik.Tests.Documents;

public class DocumentDataTests
{
    private static DocumentData Expiring(DateTime? expiry) =>
        new("012345678", "ID", "S12345", "C1", new DateTime(2020, 3, 2), expiry, "PU Beograd", "LIČNA KARTA");

    [Fact]
    public void A_document_is_valid_on_its_expiry_day_and_expired_the_day_after()
    {
        var data = Expiring(new DateTime(2030, 3, 1));

        Assert.False(data.IsExpiredOn(new DateTime(2030, 3, 1)));
        Assert.True(data.IsExpiredOn(new DateTime(2030, 3, 2)));
    }

    [Fact]
    public void A_document_without_an_expiry_date_never_expires()
    {
        Assert.False(Expiring(null).IsExpiredOn(new DateTime(2999, 1, 1)));
    }
}
