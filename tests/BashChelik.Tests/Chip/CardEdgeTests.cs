using System.Collections.Generic;
using System.Linq;
using BashChelik.Chip;
using BashChelik.Tests.Simulator;
using Xunit;

namespace BashChelik.Tests.Chip;

public class CardEdgeTests
{
    private static ChipFileReader Card(Dictionary<ushort, byte[]> cardEdge) =>
        new(new CardCommands(new CardSimulator(InventedDocument.CurrentAtr,
            new Dictionary<string, Dictionary<ushort, byte[]>> { [InventedCertificates.CardEdgeAid] = cardEdge })));

    [Fact]
    public void Both_certificates_are_read_with_their_purpose()
    {
        var certificates = CardEdge.ReadCertificates(Card(InventedCertificates.CardEdgeFiles()), TestContext.Current.CancellationToken);

        Assert.Equal(2, certificates.Count);
        var authentication = certificates.Single(c => c.Purpose == CertificatePurpose.Authentication);
        var signing = certificates.Single(c => c.Purpose == CertificatePurpose.Signing);
        Assert.Equal(InventedCertificates.Authentication.Thumbprint, authentication.Certificate.Thumbprint);
        Assert.Equal(InventedCertificates.Signing.Thumbprint, signing.Certificate.Thumbprint);
    }

    [Fact]
    public void An_applet_without_certificates_gives_an_empty_list()
    {
        Assert.Empty(CardEdge.ReadCertificates(Card(new()), TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData(new byte[] { 0x06, 0x00, 0x01, 0x00, 0x05, 0x00, 0x78, 0x9C })]
    [InlineData(new byte[] { 0x01, 0x00, 0x30, 0x00 })]
    [InlineData(new byte[] { 0x04, 0x00, 0x30, 0x82, 0x01, 0x02 })]
    public void A_file_shaped_like_a_certificate_that_is_not_one_is_a_chip_error(byte[] decoy)
    {
        var files = InventedCertificates.CardEdgeFiles();
        files[0x7104] = decoy;

        var error = Assert.Throws<DocumentReadException>(() => CardEdge.ReadCertificates(Card(files), TestContext.Current.CancellationToken));

        Assert.Equal(DocumentReadFailure.ChipError, error.Reason);
        Assert.Contains("7104", error.Message);
    }

    [Fact]
    public void Zlib_inflates_what_zlib_compressed()
    {
        var der = InventedCertificates.Signing.RawData;
        var compressed = InventedCertificates.Compressed(der).Skip(6).ToArray();

        Assert.Equal(der, Zlib.Inflate(compressed));
    }
}
