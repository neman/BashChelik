using BashChelik.Native;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.DataModel
{
    public class PortraitData
    {
        public Image<Rgba32> Portrait { get; private set; }

        public PortraitData(EID_PORTRAIT nativeData)
        {
            using (var stream = new MemoryStream(nativeData.portrait, 0, nativeData.portraitSize))
            {
                Portrait = Image.Load<Rgba32>(stream);
            }           
           
        }
    }
}
