using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;

namespace BashChelik.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var data = ElectronicIdReader.ReadAll();

                //Save citizen image from PersonalCard
                data.PortraitData.Portrait.Save("portrait.png", ImageFormat.Png);

                if (data.CertificateData != null)
                {
                    File.WriteAllBytes("certificate.cer", data.CertificateData.Certificate);
                }

                Console.WriteLine("Card Type: " + data.CardType);
                
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine($"Document Data: {data.Document}");
                Console.WriteLine($"Fixed Personal Data: {data.FixedPersonalData}");
                Console.WriteLine($"Variable Personal Data: {data.VariablePersonalData}");



                Console.WriteLine("Data loaded successfully");
            }
            catch (Exception ex)
            {
                //TODO:Add trace listener to Config file
                Trace.TraceError(ex.ToString());
                Console.WriteLine(string.Concat("Failed to load data: ", ex.ToString()));
                Console.ReadLine();
            }
        }
    }
}