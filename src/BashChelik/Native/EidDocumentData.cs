using System.Runtime.InteropServices;

namespace BashChelik.Native
{

    /// <summary>
    /// Represents the electronic identification (EID) document data used for validating and interacting with identity documents.
    /// The structure contains non-modifiable, vital document information extracted from EID cards.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct EidDocumentData
    {
        /// <summary>
        /// The document registration number as a byte array. This number is fixed and is unique to each document.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocRegNo)]
        private readonly byte[] _docRegNo;

        /// <summary>
        /// Provides access to the document registration number.
        /// </summary>
        public byte[] DocRegNo => _docRegNo;

        /// <summary>
        /// The size of the document registration number array.
        /// </summary>
        public int DocRegNoSize { get; }

        /// <summary>
        /// The type of document, represented as a byte array. This indicates the kind of document, like ID or passport.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocumentType)]
        private readonly byte[] _documentType;

        /// <summary>
        /// Provides access to the document type data.
        /// </summary>
        public byte[] DocumentType => _documentType;

        /// <summary>
        /// The size of the document type array.
        /// </summary>
        public int DocumentTypeSize { get; }

        /// <summary>
        /// The issuing date of the document, represented as a byte array.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_IssuingDate)]
        private readonly byte[] _issuingDate;

        /// <summary>
        /// Provides access to the issuing date of the document.
        /// </summary>
        public byte[] IssuingDate => _issuingDate;

        /// <summary>
        /// The size of the issuing date array.
        /// </summary>
        public int IssuingDateSize { get; }

        /// <summary>
        /// The expiry date of the document, represented as a byte array. Indicates when the document is no longer valid.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_ExpiryDate)]
        private readonly byte[] _expiryDate;

        /// <summary>
        /// Provides access to the expiry date of the document.
        /// </summary>
        public byte[] ExpiryDate => _expiryDate;

        /// <summary>
        /// The size of the expiry date array.
        /// </summary>
        public int ExpiryDateSize { get; }

        /// <summary>
        /// The issuing authority of the document, represented as a byte array. This details the entity that issued the document.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_IssuingAuthority)]
        private readonly byte[] _issuingAuthority;

        /// <summary>
        /// Provides access to the issuing authority information.
        /// </summary>
        public byte[] IssuingAuthority => _issuingAuthority;

        /// <summary>
        /// The size of the issuing authority array.
        /// </summary>
        public int IssuingAuthoritySize { get; }

        /// <summary>
        /// The serial number of the document, represented as a byte array. This number helps in further identifying the document.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocumentSerialNumber)]
        private readonly byte[] _documentSerialNumber;

        /// <summary>
        /// Provides access to the document serial number.
        /// </summary>
        public byte[] DocumentSerialNumber => _documentSerialNumber;

        /// <summary>
        /// The size of the document serial number array.
        /// </summary>
        public int DocumentSerialNumberSize { get; }

        /// <summary>
        /// The chip serial number embedded within the document, represented as a byte array. This is typically used in biometric passports.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_ChipSerialNumber)]
        private readonly byte[] _chipSerialNumber;

        /// <summary>
        /// Provides access to the chip serial number of the document.
        /// </summary>
        public byte[] ChipSerialNumber => _chipSerialNumber;

        /// <summary>
        /// The size of the chip serial number array.
        /// </summary>
        public int ChipSerialNumberSize { get; }

        /// <summary>
        /// The name of the document, represented as a byte array. This field describes the type of document, like "National ID" or "Driver's License".
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeConstants.EID_MAX_DocumentName)]
        public readonly byte[] _documentName;

        /// <summary>
        /// Provides access to the name of the document.
        /// </summary>
        public byte[] DocumentName => _documentName;

        /// <summary>
        /// The size of the document name array.
        /// </summary>
        public int DocumentNameSize { get; }
    }
}