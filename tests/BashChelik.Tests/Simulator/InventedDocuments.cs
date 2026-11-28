using System.Collections.Generic;
using System.Linq;

namespace BashChelik.Tests.Simulator;

/// <summary>A document made up for tests: what its chip holds and the values it was built from.</summary>
internal sealed class InventedDocument
{
    public InventedDocument(DocumentKind kind, string aid, Dictionary<ushort, string> records, byte[] jpeg)
    {
        Kind = kind;
        Aid = aid;
        Records = records;
        Jpeg = jpeg;
    }

    public static readonly byte[] CurrentAtr = Hex.Parse("3BF99600008031FE45534345372047434E335E");

    public DocumentKind Kind { get; }

    public string Aid { get; }

    /// <summary>Every record of files 0F02, 0F03 and 0F04, by tag.</summary>
    public Dictionary<ushort, string> Records { get; }

    public byte[] Jpeg { get; }

    public byte[] Atr => CurrentAtr;

    public Dictionary<string, Dictionary<ushort, byte[]>> Applets => new()
    {
        [Aid] = new()
        {
            [0x0F02] = File(0x0F02, 1545, 1554, 1681, 1682),
            [0x0F03] = File(0x0F03, 1555, 1567, 1582, 1583, 1683, 1684),
            [0x0F04] = File(0x0F04, 1568, 1581),
            [0x0F06] = ChipFileBuilder.Raw(0x0F06, new byte[] { 0x46, 0x06, (byte)Jpeg.Length, (byte)(Jpeg.Length >> 8) }.Concat(Jpeg).ToArray()),
        },
        [InventedCertificates.CardEdgeAid] = InventedCertificates.CardEdgeFiles(),
    };

    public CardSimulator Card() => new(Atr, Applets);

    private byte[] File(ushort headerTag, ushort from, ushort to, params ushort[] more) =>
        ChipFileBuilder.File(headerTag, Records
            .Where(r => (r.Key >= from && r.Key <= to) || more.Contains(r.Key))
            .Select(r => (r.Key, r.Value))
            .ToArray());
}

/// <summary>Invented documents, one per kind. No value belongs to a real person.</summary>
internal static class InventedDocuments
{
    public static byte[] FakeJpeg { get; } = new byte[] { 0xFF, 0xD8 }.Concat(Enumerable.Range(0, 598).Select(i => (byte)i)).ToArray();

    public static InventedDocument IdentityCard() => new(DocumentKind.IdentityCard, "F381000002534552494401", new()
    {
        [1546] = "012345678", [1547] = "ID", [1548] = "S0000001", [1549] = "02032020", [1550] = "01032030",
        [1551] = "PU Beograd", [1681] = "C0000001", [1682] = "LIČNA KARTA",
        [1558] = "0101990710008", [1559] = "Ćirić", [1560] = "Žarko", [1561] = "Đorđe", [1562] = "M",
        [1563] = "Beograd", [1564] = "Vračar", [1565] = "Srbija", [1566] = "01011990", [1567] = "SRB",
        [1568] = "Srbija", [1569] = "Vračar", [1570] = "Beograd", [1571] = "Njegoševa", [1572] = "12",
        [1573] = "a", [1574] = "2", [1575] = "3", [1578] = "14", [1580] = "05062021", [1581] = "BG-123",
    }, FakeJpeg);

    public static InventedDocument ForeignerIdentityCard() => new(DocumentKind.ForeignerIdentityCard, "F381000002534552494601", new()
    {
        [1546] = "IF0000001", [1547] = "IF", [1548] = "S0000002", [1549] = "10102022", [1550] = "09102027",
        [1551] = "PU Novi Sad",
        [1558] = "1505005715006", [1559] = "Smith", [1560] = "Anna", [1562] = "Ž",
        [1563] = "London", [1565] = "United Kingdom", [1566] = "00001975", [1567] = "GBR",
        [1582] = "stalno nastanjen stranac", [1583] = "britansko", [1683] = "rad", [1684] = "napomena",
        [1568] = "Srbija", [1569] = "Novi Sad", [1570] = "Novi Sad", [1571] = "Bulevar", [1572] = "1",
        [1580] = "01112022",
    }, FakeJpeg);

    public static InventedDocument ResidencePermit() => new(DocumentKind.ResidencePermit, "F381000002534552525001", new()
    {
        [1546] = "RP0000001", [1547] = "RP", [1548] = "S0000003", [1549] = "15012024", [1550] = "01010001",
        [1551] = "PU Niš",
        [1558] = "2002998750004", [1559] = "Müller", [1560] = "Jörg", [1562] = "M",
        [1563] = "Berlin", [1565] = "Deutschland", [1566] = "20021998", [1567] = "DEU",
        [1583] = "nemačko", [1683] = "studije",
        [1569] = "Niš", [1570] = "Niš", [1571] = "Obrenovićeva", [1572] = "7", [1580] = "20012024",
    }, FakeJpeg);
}
