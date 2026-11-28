using System;
using System.Collections.Generic;
using System.Linq;
using BashChelik.Chip;

namespace BashChelik.ConsoleApp;

/// <summary>Manual test tool: reads a document from a card reader and prints it on screen.</summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var command = args.FirstOrDefault();
        var rest = args.Skip(1).ToList();
        var portraitPath = TakeOption(rest, "--portrait");
        if (portraitPath == string.Empty)
        {
            Console.Error.WriteLine("--portrait needs a file path, for example --portrait C:\\Temp\\portrait.jpg");
            return 1;
        }

        var readerName = rest.FirstOrDefault() ?? PcscCardChannel.ListReaders().FirstOrDefault();
        if (command is "probe" or "read" && readerName is null)
        {
            Console.Error.WriteLine("No card reader is connected.");
            return 2;
        }

        try
        {
            return command switch
            {
                "probe" => Probe.Run(readerName!),
                "read" => Read.Run(readerName!, portraitPath),
                _ => Usage(),
            };
        }
        catch (DocumentReadException e)
        {
            Console.Error.WriteLine($"{e.Reason}: {e.Message}");
            return 3;
        }
    }

    /// <summary>Removes <paramref name="name"/> and its value from <paramref name="args"/>; empty when the value is missing.</summary>
    private static string? TakeOption(List<string> args, string name)
    {
        var at = args.IndexOf(name);
        if (at < 0)
            return null;
        var value = at + 1 < args.Count ? args[at + 1] : string.Empty;
        args.RemoveRange(at, value.Length == 0 ? 1 : 2);
        return value;
    }

    private static int Usage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  read [reader] [--portrait <file.jpg>]   print the document in the reader;");
        Console.WriteLine("                                         --portrait also saves the photo to that file");
        Console.WriteLine("  probe [reader]                          print the structure of the chip, without any value");
        return 1;
    }
}
