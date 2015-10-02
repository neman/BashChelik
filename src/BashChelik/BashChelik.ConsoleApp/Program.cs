﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var data = ElectronicIdReader.ReadAll();
                
                //Save citizen image from PersonalCard
                data.PortraitData.Portrait.Save("portrait.png", ImageFormat.Png);

                File.WriteAllBytes("certificate.cer", data.CertificateData.Certificate);
            }
            catch (Exception ex)
            {
                //TODO:Add trace listener to Config file
                Trace.TraceError(ex.ToString());
            }
            
        }
    }
}
