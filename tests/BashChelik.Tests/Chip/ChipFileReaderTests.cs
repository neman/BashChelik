using System.Collections.Generic;
using System.Linq;
using BashChelik.Chip;
using BashChelik.Tests.Simulator;
using Xunit;

namespace BashChelik.Tests.Chip;

public class ChipFileReaderTests
{
    private const string IdentityCardAid = "F381000002534552494401";

    private static byte[] FileWithContent(int length)
    {
        var content = Enumerable.Range(0, length).Select(i => (byte)(i * 7)).ToArray();
        return new byte[] { 0x02, 0x0F, (byte)length, (byte)(length >> 8) }.Concat(content).ToArray();
    }

    private static ChipFileReader Reader(Dictionary<ushort, byte[]> files) =>
        new(new CardCommands(new CardSimulator(new byte[] { 0x3B },
            new Dictionary<string, Dictionary<ushort, byte[]>> { [IdentityCardAid] = files })));

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(251)]
    [InlineData(255)]
    [InlineData(256)]
    [InlineData(300)]
    [InlineData(510)]
    [InlineData(511)]
    [InlineData(0x8000 - 4)]
    public void ReadFile_returns_header_and_content(int length)
    {
        var stored = FileWithContent(length);
        var reader = Reader(new() { [0x0F02] = stored });
        reader.SelectApplet(Hex.Parse(IdentityCardAid), TestContext.Current.CancellationToken);

        var file = reader.ReadFile(0x0F02, TestContext.Current.CancellationToken);

        Assert.Equal((ushort)0x0F02, file.Fid);
        Assert.Equal((ushort)0x0F02, file.HeaderTag);
        Assert.Equal(length, file.DeclaredLength);
        Assert.Equal(4 + length, file.Raw.Length);
        Assert.Equal(stored, file.Raw);
        Assert.Equal(stored.Skip(4), file.Content);
    }

    [Fact]
    public void ReadFile_rejects_a_declared_length_outside_the_address_range()
    {
        var reader = Reader(new() { [0x0F02] = new byte[] { 0x02, 0x0F, 0xFD, 0x7F } });
        reader.SelectApplet(Hex.Parse(IdentityCardAid), TestContext.Current.CancellationToken);

        var error = Assert.Throws<DocumentReadException>(() => reader.ReadFile(0x0F02, TestContext.Current.CancellationToken));

        Assert.Equal(DocumentReadFailure.ChipError, error.Reason);
        Assert.Contains("address range", error.Message);
    }

    [Fact]
    public void TrySelectApplet_is_false_for_an_absent_applet()
    {
        var reader = Reader(new());

        Assert.False(reader.TrySelectApplet(Hex.Parse("F381000002534552525001"), TestContext.Current.CancellationToken));
        Assert.True(reader.TrySelectApplet(Hex.Parse(IdentityCardAid), TestContext.Current.CancellationToken));
    }
}
