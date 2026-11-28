using System;
using System.Linq;

namespace BashChelik.Tests.Simulator;

internal static class Hex
{
    public static byte[] Parse(string hex) =>
        Enumerable.Range(0, hex.Length / 2).Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16)).ToArray();
}
