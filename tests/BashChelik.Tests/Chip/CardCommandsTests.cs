using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BashChelik.Chip;
using BashChelik.Tests.Simulator;
using Xunit;

namespace BashChelik.Tests.Chip;

public class CardCommandsTests
{
    private static readonly byte[] Aid = { 0xF3, 0x81, 0x00, 0x00, 0x02, 0x53, 0x45, 0x52, 0x49, 0x44, 0x01 };
    private static readonly byte[] File = Enumerable.Range(0, 40).Select(i => (byte)i).ToArray();

    private static (CardSimulator Card, CardCommands Commands) SelectedFile(bool t0)
    {
        var card = new CardSimulator(new byte[] { 0x3B }, new Dictionary<string, Dictionary<ushort, byte[]>>
        {
            ["F381000002534552494401"] = new() { [0x0F02] = File },
        }) { T0Mode = t0 };
        var commands = new CardCommands(card);
        commands.SendExpectingSuccess(Apdu.SelectAid(Aid), TestContext.Current.CancellationToken);
        commands.SendExpectingSuccess(Apdu.SelectFile(0x0F02), TestContext.Current.CancellationToken);
        return (card, commands);
    }

    [Fact]
    public void Send_follows_61xx_with_get_response()
    {
        var (_, commands) = SelectedFile(t0: true);

        var response = commands.Send(Apdu.ReadBinary(0, 16), TestContext.Current.CancellationToken);

        Assert.Equal(0x9000, response.StatusWord);
        Assert.Equal(File.Take(16), response.Data);
    }

    [Fact]
    public void Send_resends_with_le_on_6Cxx()
    {
        var (_, commands) = SelectedFile(t0: true);

        var response = commands.Send(Apdu.ReadBinary(30, 0xFF), TestContext.Current.CancellationToken);

        Assert.Equal(0x9000, response.StatusWord);
        Assert.Equal(File.Skip(30), response.Data);
    }

    [Fact]
    public void Send_keeps_all_chained_data_and_corrects_get_response_length()
    {
        var select = Apdu.SelectFile(0x0F02);
        var middle = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
        using var card = new ScriptedCard(
            (select, new ResponseApdu(new byte[] { 0x42 }, 0x6100)),
            (Apdu.GetResponse(0), new ResponseApdu(middle, 0x6103)),
            (Apdu.GetResponse(3), new ResponseApdu(Array.Empty<byte>(), 0x6C02)),
            (Apdu.GetResponse(2), new ResponseApdu(new byte[] { 0x12, 0x34 }, 0x9000)));
        var commands = new CardCommands(card);

        var response = commands.Send(select, TestContext.Current.CancellationToken);

        Assert.Equal(0x9000, response.StatusWord);
        Assert.Equal(new byte[] { 0x42 }.Concat(middle).Concat(new byte[] { 0x12, 0x34 }), response.Data);
    }

    [Fact]
    public void Send_returns_6Cxx_for_select_without_le()
    {
        var select = Apdu.SelectFile(0x0F02);
        using var card = new ScriptedCard((select, new ResponseApdu(Array.Empty<byte>(), 0x6C04)));
        var commands = new CardCommands(card);

        var response = commands.Send(select, TestContext.Current.CancellationToken);

        Assert.Equal(0x6C04, response.StatusWord);
    }

    [Fact]
    public void SendExpectingSuccess_throws_chip_error_with_status_word()
    {
        var (_, commands) = SelectedFile(t0: false);

        var error = Assert.Throws<DocumentReadException>(() => commands.SendExpectingSuccess(Apdu.SelectFile(0x0F99), TestContext.Current.CancellationToken));

        Assert.Equal(DocumentReadFailure.ChipError, error.Reason);
        Assert.Equal((ushort)0x6A82, error.StatusWord);
    }

    [Fact]
    public void Send_stops_when_cancelled()
    {
        var (card, commands) = SelectedFile(t0: false);
        var sent = card.Commands.Count;

        Assert.ThrowsAny<OperationCanceledException>(() => commands.Send(Apdu.ReadBinary(0, 16), new CancellationToken(canceled: true)));
        Assert.Equal(sent, card.Commands.Count);
    }

    private sealed class ScriptedCard(params (byte[] Command, ResponseApdu Response)[] exchanges) : ICardChannel
    {
        private readonly Queue<(byte[] Command, ResponseApdu Response)> _exchanges = new(exchanges);

        public byte[] Atr => Array.Empty<byte>();

        public ResponseApdu Transmit(byte[] command)
        {
            var exchange = _exchanges.Dequeue();
            Assert.Equal(exchange.Command, command);
            return exchange.Response;
        }

        public IDisposable BeginTransaction() => throw new NotSupportedException();

        public void Dispose() => Assert.Empty(_exchanges);
    }
}
