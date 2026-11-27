using System;
using System.Collections.Generic;
using BashChelik.Chip;
using BashChelik.Tests.Simulator;
using Xunit;

namespace BashChelik.Tests.Chip;

public class LayoutTests
{
    public static TheoryData<string> Kinds => new() { "identity card", "foreigner identity card", "residence permit" };

    private static InventedDocument Invented(string kind) => kind switch
    {
        "identity card" => InventedDocuments.IdentityCard(),
        "foreigner identity card" => InventedDocuments.ForeignerIdentityCard(),
        _ => InventedDocuments.ResidencePermit(),
    };

    private static ChipFile Read(InventedDocument document, ushort fid)
    {
        var files = new ChipFileReader(new CardCommands(document.Card()));
        files.SelectApplet(Hex.Parse(document.Aid), TestContext.Current.CancellationToken);
        return files.ReadFile(fid, TestContext.Current.CancellationToken);
    }

    private static string? Value(InventedDocument document, ushort tag) =>
        document.Records.TryGetValue(tag, out var value) ? value : null;

    [Theory]
    [MemberData(nameof(Kinds))]
    public void Document_data_maps_every_field(string kind)
    {
        var document = Invented(kind);

        var data = DocumentDataLayout.Read(TagRecords.Parse(Read(document, DocumentDataLayout.Fid).Content));

        Assert.Equal(Value(document, 1546), data.Number);
        Assert.Equal(Value(document, 1547), data.TypeCode);
        Assert.Equal(Value(document, 1548), data.SerialNumber);
        Assert.Equal(ChipText.Date(Value(document, 1549)), data.IssueDate);
        Assert.Equal(ChipText.Date(Value(document, 1550)), data.ExpiryDate);
        Assert.Equal(Value(document, 1551), data.IssuingAuthority);
        Assert.Equal(Value(document, 1681), data.ChipSerialNumber);
        Assert.Equal(Value(document, 1682), data.DocumentName);
    }

    [Fact]
    public void Dates_are_read_from_the_chip_text()
    {
        var data = DocumentDataLayout.Read(TagRecords.Parse(Read(InventedDocuments.IdentityCard(), DocumentDataLayout.Fid).Content));

        Assert.Equal(new DateTime(2020, 3, 2), data.IssueDate);
        Assert.Equal(new DateTime(2030, 3, 1), data.ExpiryDate);
    }

    [Fact]
    public void A_placeholder_expiry_date_is_null()
    {
        var data = DocumentDataLayout.Read(TagRecords.Parse(Read(InventedDocuments.ResidencePermit(), DocumentDataLayout.Fid).Content));

        Assert.Null(data.ExpiryDate);
    }

    [Theory]
    [MemberData(nameof(Kinds))]
    public void Personal_data_maps_every_field(string kind)
    {
        var document = Invented(kind);

        var person = PersonalDataLayout.Read(TagRecords.Parse(Read(document, PersonalDataLayout.Fid).Content), document.Kind);

        Assert.Equal(Value(document, 1558), person.PersonalNumber?.Value);
        Assert.Equal(Value(document, 1559), person.Surname);
        Assert.Equal(Value(document, 1560), person.GivenName);
        Assert.Equal(Value(document, 1561), person.ParentGivenName);
        Assert.Equal(Value(document, 1562), person.Sex);
        Assert.Equal(Value(document, 1563), person.PlaceOfBirth);
        Assert.Equal(Value(document, 1564), person.CommunityOfBirth);
        Assert.Equal(Value(document, 1565), person.StateOfBirth);
        Assert.Equal(Value(document, 1567), person.StateOfBirthCode);
        Assert.Equal(ChipText.BirthDate(Value(document, 1566))?.Year, person.BirthDate?.Year);
    }

    [Fact]
    public void An_identity_card_has_no_foreigner_data()
    {
        var document = InventedDocuments.IdentityCard();

        var person = PersonalDataLayout.Read(TagRecords.Parse(Read(document, PersonalDataLayout.Fid).Content), document.Kind);

        Assert.Null(person.ForeignerData);
    }

    [Fact]
    public void A_foreigner_identity_card_has_foreigner_data()
    {
        var document = InventedDocuments.ForeignerIdentityCard();

        var foreigner = PersonalDataLayout.Read(TagRecords.Parse(Read(document, PersonalDataLayout.Fid).Content), document.Kind).ForeignerData!;

        Assert.Equal("britansko", foreigner.Nationality);
        Assert.Equal("stalno nastanjen stranac", foreigner.Status);
        Assert.Equal("rad", foreigner.PurposeOfStay);
        Assert.Equal("napomena", foreigner.Note);
    }

    [Fact]
    public void A_birth_date_without_day_and_month_keeps_its_year()
    {
        var document = InventedDocuments.ForeignerIdentityCard();

        var birth = PersonalDataLayout.Read(TagRecords.Parse(Read(document, PersonalDataLayout.Fid).Content), document.Kind).BirthDate!;

        Assert.Equal(1975, birth.Year);
        Assert.Null(birth.Month);
    }

    [Theory]
    [MemberData(nameof(Kinds))]
    public void Address_maps_every_field(string kind)
    {
        var document = Invented(kind);

        var address = AddressLayout.Read(TagRecords.Parse(Read(document, AddressLayout.Fid).Content));

        Assert.Equal(Value(document, 1568), address.State);
        Assert.Equal(Value(document, 1569), address.Community);
        Assert.Equal(Value(document, 1570), address.Place);
        Assert.Equal(Value(document, 1571), address.Street);
        Assert.Equal(Value(document, 1572), address.HouseNumber);
        Assert.Equal(Value(document, 1573), address.HouseLetter);
        Assert.Equal(Value(document, 1574), address.Entrance);
        Assert.Equal(Value(document, 1575), address.Floor);
        Assert.Equal(Value(document, 1578), address.ApartmentNumber);
        Assert.Equal(ChipText.Date(Value(document, 1580)), address.AddressDate);
        Assert.Equal(Value(document, 1581), address.Label);
    }

    [Fact]
    public void A_residence_permit_address_has_no_state()
    {
        var address = AddressLayout.Read(TagRecords.Parse(Read(InventedDocuments.ResidencePermit(), AddressLayout.Fid).Content));

        Assert.Null(address.State);
        Assert.Equal("Niš", address.Place);
    }

    [Fact]
    public void The_portrait_is_the_jpeg_after_the_inner_header()
    {
        var portrait = PortraitLayout.Read(Read(InventedDocuments.IdentityCard(), PortraitLayout.Fid));

        Assert.Equal(InventedDocuments.FakeJpeg, portrait.ToJpeg());
    }

    [Fact]
    public void A_portrait_that_is_not_a_jpeg_is_a_chip_error()
    {
        var notJpeg = ChipFileBuilder.Raw(0x0F06, new byte[] { 0x46, 0x06, 0x02, 0x00, 0x00, 0x00 });

        var error = Assert.Throws<DocumentReadException>(() => PortraitLayout.Read(new ChipFile(0x0F06, notJpeg)));

        Assert.Equal(DocumentReadFailure.ChipError, error.Reason);
    }
}
