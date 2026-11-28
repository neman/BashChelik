using Xunit;

namespace BashChelik.Tests.Documents;

public class DocumentKindTests
{
    [Fact]
    public void Each_kind_says_which_data_it_has()
    {
        Assert.False(DocumentKind.IdentityCard.HasForeignerData);
        Assert.True(DocumentKind.ForeignerIdentityCard.HasForeignerData);
        Assert.True(DocumentKind.ResidencePermit.HasForeignerData);
        Assert.True(DocumentKind.IdentityCard.AddressHasState);
        Assert.False(DocumentKind.ResidencePermit.AddressHasState);
    }
}
