using System.Runtime.InteropServices;

namespace BashChelik.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct EID_DOCUMENT_DATA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocRegNo)]
        private readonly byte[] _docRegNo;

        public byte[] DocRegNo => _docRegNo;
        public int DocRegNoSize { get; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocumentType)]
        private readonly byte[] _documentType;

        public byte[] DocumentType => _documentType;
        public int DocumentTypeSize { get; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_IssuingDate)]
        private readonly byte[] _issuingDate;

        public byte[] IssuingDate => _issuingDate;
        public int IssuingDateSize { get; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_ExpiryDate)]
        private readonly byte[] _expiryDate;

        public byte[] ExpiryDate => _expiryDate;

        public int ExpiryDateSize { get; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_IssuingAuthority)]
        private readonly byte[] _issuingAuthority;

        public byte[] IssuingAuthority => _issuingAuthority;
        public int IssuingAuthoritySize { get; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocumentSerialNumber)]
        private readonly byte[] _documentSerialNumber;

        public byte[] DocumentSerialNumber => _documentSerialNumber;
        public int DocumentSerialNumberSize { get; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_ChipSerialNumber)]
        private readonly byte[] _chipSerialNumber;

        public byte[] ChipSerialNumber => _chipSerialNumber;
        public int ChipSerialNumberSize { get; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocumentName)]
        public readonly byte[] _documentName;

        public byte[] DocumentName => _documentName;
        public int DocumentNameSize { get; }
    }
}