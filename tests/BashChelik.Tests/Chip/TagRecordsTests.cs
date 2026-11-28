using System.Collections.Generic;
using System.Linq;
using BashChelik.Chip;
using BashChelik.Tests.Simulator;
using Xunit;

namespace BashChelik.Tests.Chip;

public class TagRecordsTests
{
    private static byte[] Content(params (ushort Tag, string Value)[] records) =>
        ChipFileBuilder.File(0x0F02, records).Skip(4).ToArray();

    [Fact]
    public void Parse_skips_unknown_tags()
    {
        var records = TagRecords.Parse(Content((1545, "x"), (1546, "012345678"), (1552, "y")));

        Assert.Equal("012345678", records.Text(1546));
        Assert.Null(records.Text(1681));
    }

    [Fact]
    public void An_empty_value_reads_as_null()
    {
        Assert.Null(TagRecords.Parse(Content((1573, ""))).Text(1573));
    }

    [Fact]
    public void The_first_occurrence_of_a_tag_wins()
    {
        Assert.Equal("first", TagRecords.Parse(Content((1559, "first"), (1559, "second"))).Text(1559));
    }

    [Fact]
    public void Text_decoded_after_whole_file()
    {
        // The record value starts at raw offset 258, so "Ž" (2 bytes in UTF-8) sits at raw offsets 258 and 259,
        // across the end of the first content chunk (raw offsets 4 to 258).
        var padding = new string('a', 246);
        var stored = ChipFileBuilder.File(0x0F03, (1545, padding), (1559, "Žarko Ćirić"));
        var reader = new ChipFileReader(new CardCommands(new CardSimulator(new byte[] { 0x3B },
            new Dictionary<string, Dictionary<ushort, byte[]>> { ["F381000002534552494401"] = new() { [0x0F03] = stored } })));
        reader.SelectApplet(Hex.Parse("F381000002534552494401"), TestContext.Current.CancellationToken);

        var records = TagRecords.Parse(reader.ReadFile(0x0F03, TestContext.Current.CancellationToken).Content);

        Assert.Equal("Žarko Ćirić", records.Text(1559));
    }
}
