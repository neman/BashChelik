using System;
using Xunit;

namespace BashChelik.Tests.Documents;

public class IdentityDocumentTests
{
    private static IdentityDocument Issued(DateTime issued, DateTime expires) => new(
        DocumentKind.IdentityCard,
        new DocumentData("012345678", "ID", "S12345", "C1", issued, expires, "PU Beograd", "LIČNA KARTA"),
        new PersonalData(new PersonalNumber("0101990710008"), "Perić", "Pera", "Petar", "M",
            new BirthDate(1990, 1, 1), "Beograd", "Vračar", "Srbija", "SRB", null),
        new Address("Srbija", "Vračar", "Beograd", "Njegoševa", "1", null, null, null, null, issued, null),
        new Portrait(new byte[] { 0xFF, 0xD8 }),
        Array.Empty<HolderCertificate>());

    [Fact]
    public void A_document_is_valid_between_its_issue_and_expiry_days()
    {
        var document = Issued(new DateTime(2020, 3, 2), new DateTime(2030, 3, 1));

        Assert.False(document.IsValidOn(new DateTime(2020, 3, 1)));
        Assert.True(document.IsValidOn(new DateTime(2020, 3, 2)));
        Assert.True(document.IsValidOn(new DateTime(2030, 3, 1)));
        Assert.False(document.IsValidOn(new DateTime(2030, 3, 2)));
    }

    [Fact]
    public void The_portrait_cannot_be_changed_through_a_copy()
    {
        var portrait = new Portrait(new byte[] { 0xFF, 0xD8, 0x01 });

        portrait.ToJpeg()[2] = 0x99;

        Assert.Equal(new byte[] { 0xFF, 0xD8, 0x01 }, portrait.ToJpeg());
    }
}
