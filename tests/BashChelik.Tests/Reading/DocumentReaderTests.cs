using System.Linq;
using BashChelik.Tests.Simulator;
using Xunit;

namespace BashChelik.Tests.Reading;

public class DocumentReaderTests
{
    public static TheoryData<string> Kinds => new() { "identity card", "foreigner identity card", "residence permit" };

    private static InventedDocument Invented(string kind) => kind switch
    {
        "identity card" => InventedDocuments.IdentityCard(),
        "foreigner identity card" => InventedDocuments.ForeignerIdentityCard(),
        _ => InventedDocuments.ResidencePermit(),
    };

    [Theory]
    [MemberData(nameof(Kinds))]
    public void Open_tells_the_kind(string kind)
    {
        var invented = Invented(kind);

        using var reader = DocumentReader.Open(invented.Card(), TestContext.Current.CancellationToken);

        Assert.Same(invented.Kind, reader.Kind);
    }

    [Fact]
    public void ReadDocument_returns_all_parts_and_evidence()
    {
        var invented = InventedDocuments.IdentityCard();
        using var reader = DocumentReader.Open(invented.Card(), TestContext.Current.CancellationToken);

        var document = reader.ReadDocument(TestContext.Current.CancellationToken);

        Assert.Same(DocumentKind.IdentityCard, document.Kind);
        Assert.Equal("012345678", document.DocumentData.Number);
        Assert.Equal("Ćirić", document.PersonalData.Surname);
        Assert.Equal("Njegoševa", document.Address.Street);
        Assert.Equal(InventedDocuments.FakeJpeg, document.Portrait.ToJpeg());
        var files = invented.Applets[invented.Aid];
        Assert.Equal(new ushort[] { 0x0F02, 0x0F03, 0x0F04, 0x0F06 }, document.Evidence.Keys.OrderBy(k => k));
        foreach (var fid in document.Evidence.Keys)
            Assert.Equal(files[fid], document.Evidence[fid]);
    }

    [Fact]
    public void Each_part_can_be_read_on_its_own()
    {
        using var reader = DocumentReader.Open(InventedDocuments.ForeignerIdentityCard().Card(), TestContext.Current.CancellationToken);

        Assert.Equal("IF", reader.ReadDocumentData(TestContext.Current.CancellationToken).TypeCode);
        Assert.Equal("britansko", reader.ReadPersonalData(TestContext.Current.CancellationToken).ForeignerData!.Nationality);
        Assert.Equal("Bulevar", reader.ReadAddress(TestContext.Current.CancellationToken).Street);
        Assert.Equal(InventedDocuments.FakeJpeg, reader.ReadPortrait(TestContext.Current.CancellationToken).ToJpeg());
    }

    [Fact]
    public void Card_removed_mid_read_throws_CardRemoved()
    {
        var card = InventedDocuments.IdentityCard().Card();
        using var reader = DocumentReader.Open(card, TestContext.Current.CancellationToken);
        card.RemoveAfterCommands(5);

        var error = Assert.Throws<DocumentReadException>(() => reader.ReadDocument(TestContext.Current.CancellationToken));

        Assert.Equal(DocumentReadFailure.CardRemoved, error.Reason);
    }

    [Fact]
    public void Reads_work_in_T0_mode()
    {
        var card = InventedDocuments.ResidencePermit().Card();
        card.T0Mode = true;
        using var reader = DocumentReader.Open(card, TestContext.Current.CancellationToken);

        var document = reader.ReadDocument(TestContext.Current.CancellationToken);

        Assert.Equal("Müller", document.PersonalData.Surname);
        Assert.Equal(InventedDocuments.FakeJpeg, document.Portrait.ToJpeg());
    }

    [Fact]
    public void Each_read_is_one_transaction()
    {
        var card = InventedDocuments.IdentityCard().Card();
        using var reader = DocumentReader.Open(card, TestContext.Current.CancellationToken);
        var before = card.TransactionsBegun;

        reader.ReadDocument(TestContext.Current.CancellationToken);
        reader.ReadAddress(TestContext.Current.CancellationToken);

        Assert.Equal(before + 2, card.TransactionsBegun);
        Assert.Equal(card.TransactionsBegun, card.TransactionsEnded);
    }

    [Fact]
    public void A_transaction_ends_when_a_read_fails()
    {
        var card = InventedDocuments.IdentityCard().Card();
        using var reader = DocumentReader.Open(card, TestContext.Current.CancellationToken);
        card.RemoveAfterCommands(1);

        Assert.Throws<DocumentReadException>(() => reader.ReadPortrait(TestContext.Current.CancellationToken));

        Assert.Equal(card.TransactionsBegun, card.TransactionsEnded);
    }
}
