using BashChelik.Native;
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
        public Bitmap Portrait { get; private set; }

        public PortraitData(EID_PORTRAIT nativeData)
        {
            using (var stream = new MemoryStream(nativeData.portrait))
            using (var image = Image.FromStream(stream, false, true))
            {
                Portrait = new Bitmap(image);
            }
           
        }
    }
}
