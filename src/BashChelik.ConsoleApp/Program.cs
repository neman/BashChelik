using System;
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
        var readerName = args.Skip(1).FirstOrDefault() ?? PcscCardChannel.ListReaders().FirstOrDefault();

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
                _ => Usage(),
            };
        }
        catch (DocumentReadException e)
        {
            Console.Error.WriteLine($"{e.Reason}: {e.Message}");
            return 3;
        }
    }

    private static int Usage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  read [reader]    print the document in the reader");
        Console.WriteLine("  probe [reader]   print the structure of the chip, without any value");
        return 1;
    }
}
