using System;

namespace BashChelik.ConsoleApp;

/// <summary>Manual test tool: reads a document from a card reader and prints it on screen.</summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Usage:");
        Console.WriteLine("  read [reader]    print the document in the reader");
        Console.WriteLine("  probe [reader]   print the structure of the chip, without any value");
        return 1;
    }
}
