using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EID_CERTIFICATE
    {

        /// BYTE[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_Certificate)]
        public byte[] certificate;

        /// int
        public int certificateSize;
    }
}
