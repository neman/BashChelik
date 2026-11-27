using System;
using System.Collections.Generic;
using System.Linq;
using BashChelik.Chip;

namespace BashChelik.Tests.Simulator;

/// <summary>
/// A chip that answers the commands the reader sends, from invented files.
/// Applets are keyed by AID in hex; each holds files keyed by FID, as the chip stores them (header included).
/// </summary>
internal sealed class CardSimulator : ICardChannel
{
    private readonly Dictionary<string, Dictionary<ushort, byte[]>> _applets;
    private Dictionary<ushort, byte[]>? _applet;
    private byte[]? _file;
    private byte[]? _pendingResponse;
    private int? _commandsUntilRemoved;

    public CardSimulator(byte[] atr, Dictionary<string, Dictionary<ushort, byte[]>> applets)
    {
        Atr = atr;
        _applets = new Dictionary<string, Dictionary<ushort, byte[]>>(applets, StringComparer.OrdinalIgnoreCase);
    }

    public byte[] Atr { get; }

    /// <summary>Answer like a T=0 reader: <c>61xx</c> before data, <c>6Cxx</c> when Le is too long.</summary>
    public bool T0Mode { get; set; }

    public List<byte[]> Commands { get; } = new();

    public int TransactionsBegun { get; private set; }

    public int TransactionsEnded { get; private set; }

    /// <summary>After <paramref name="commands"/> more commands, every command fails as if the card was pulled out.</summary>
    public void RemoveAfterCommands(int commands) => _commandsUntilRemoved = commands;

    public ResponseApdu Transmit(byte[] command)
    {
        if (_commandsUntilRemoved is int left)
        {
            if (left <= 0)
                throw new DocumentReadException(DocumentReadFailure.CardRemoved, "The card was removed.");
            _commandsUntilRemoved = left - 1;
        }

        Commands.Add(command);
        byte ins = command[1], p1 = command[2];

        if (ins == 0xC0)
            return GetResponse();
        if (ins == 0xA4 && p1 == 0x04)
            return SelectApplet(command);
        if (ins == 0xA4 && p1 == 0x08)
            return SelectFile(command);
        if (ins == 0xB0)
            return ReadBinary(command);
        return Status(0x6D00);
    }

    public IDisposable BeginTransaction()
    {
        TransactionsBegun++;
        return new Transaction(this);
    }

    public void Dispose()
    {
    }

    private ResponseApdu SelectApplet(byte[] command)
    {
        var aid = Hex(command.Skip(5).Take(command[4]));
        if (!_applets.TryGetValue(aid, out var applet))
            return Status(0x6700);
        _applet = applet;
        _file = null;
        return Status(0x9000);
    }

    private ResponseApdu SelectFile(byte[] command)
    {
        var fid = (ushort)((command[5] << 8) | command[6]);
        if (_applet is null || !_applet.TryGetValue(fid, out var file))
            return Status(0x6A82);
        _file = file;
        return Status(0x9000);
    }

    private ResponseApdu ReadBinary(byte[] command)
    {
        if (_file is null)
            return Status(0x6986);
        int offset = (command[2] << 8) | command[3];
        int le = command.Length > 4 ? (command[4] == 0 ? 256 : command[4]) : 256;
        if (offset > _file.Length)
            return Status(0x6B00);

        int available = Math.Min(le, _file.Length - offset);
        var data = _file.Skip(offset).Take(available).ToArray();
        if (!T0Mode)
            return new ResponseApdu(data, 0x9000);
        if (available < le)
            return Status((ushort)(0x6C00 | available));
        _pendingResponse = data;
        return Status((ushort)(0x6100 | (data.Length & 0xFF)));
    }

    private ResponseApdu GetResponse()
    {
        if (_pendingResponse is null)
            return Status(0x6985);
        var data = _pendingResponse;
        _pendingResponse = null;
        return new ResponseApdu(data, 0x9000);
    }

    private static ResponseApdu Status(ushort statusWord) => new(Array.Empty<byte>(), statusWord);

    private static string Hex(IEnumerable<byte> bytes) => string.Concat(bytes.Select(b => b.ToString("X2")));

    private sealed class Transaction(CardSimulator card) : IDisposable
    {
        private bool _ended;

        public void Dispose()
        {
            if (_ended)
                return;
            _ended = true;
            card.TransactionsEnded++;
        }
    }
}
