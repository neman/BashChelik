using System.Collections.Generic;
using BashChelik.Chip;
using BashChelik.Tests.Simulator;
using Xunit;

namespace BashChelik.Tests.Chip;

public class DocumentRecognizerTests
{
    private static readonly byte[] CurrentAtr = Hex.Parse("3BF99600008031FE45534345372047434E335E");

    private static ChipFileReader Card(byte[] atr, params string[] aids)
    {
        var applets = new Dictionary<string, Dictionary<ushort, byte[]>>();
        foreach (var aid in aids)
            applets[aid] = new();
        return new ChipFileReader(new CardCommands(new CardSimulator(atr, applets)));
    }

    [Theory]
    [InlineData("F381000002534552494401", "Identity card")]
    [InlineData("F381000002534552494601", "Foreigner identity card")]
    [InlineData("F381000002534552525001", "Residence permit")]
    public void The_applet_that_answers_tells_the_kind(string aid, string kind)
    {
        Assert.Equal(kind, DocumentRecognizer.Recognize(Card(CurrentAtr, aid), TestContext.Current.CancellationToken).Name);
    }

    [Fact]
    public void A_2008_identity_card_is_refused_by_its_atr()
    {
        var error = Assert.Throws<DocumentReadException>(() => DocumentRecognizer.Recognize(
            Card(Hex.Parse("3BB918008131FE9E8073FF614083000000DF"), "F381000002534552494401"), TestContext.Current.CancellationToken));

        Assert.Equal(DocumentReadFailure.UnsupportedDocument, error.Reason);
        Assert.Contains("2008 identity card", error.Message);
    }

    [Fact]
    public void A_service_identity_card_is_refused()
    {
        var error = Assert.Throws<DocumentReadException>(() => DocumentRecognizer.Recognize(
            Card(CurrentAtr, "F381000002534552454C01"), TestContext.Current.CancellationToken));

        Assert.Equal(DocumentReadFailure.UnsupportedDocument, error.Reason);
        Assert.Contains("service identity card", error.Message);
    }

    [Fact]
    public void A_card_with_no_known_applet_is_refused()
    {
        var error = Assert.Throws<DocumentReadException>(() => DocumentRecognizer.Recognize(Card(CurrentAtr), TestContext.Current.CancellationToken));

        Assert.Equal(DocumentReadFailure.UnsupportedDocument, error.Reason);
        Assert.Contains("unknown document", error.Message);
    }
}
