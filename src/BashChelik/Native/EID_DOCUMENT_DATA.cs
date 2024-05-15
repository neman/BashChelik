using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EID_DOCUMENT_DATA
    {
        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocRegNo)]
        public byte[] docRegNo;

        /// int
        public int docRegNoSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocumentType)]
        public byte[] documentType;

        /// int
        public int documentTypeSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_IssuingDate)]
        public byte[] issuingDate;

        ///int
        public int issuingDateSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_ExpiryDate)]
        public byte[] expiryDate;

        /// int
        public int expiryDateSize;

        /// char[]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_IssuingAuthority)]
        public byte[] issuingAuthority;

        /// int
        public int issuingAuthoritySize;             
    }

    
}
