using BashChelik.Native;
using System;
using System.Text;

namespace BashChelik.DataModel
{
    public class DocumentData
    {
        private DocumentData()
        { }

        public string DocumentRegistrationNumber { get; private set; }
        public string DocumentType { get; private set; }
        public string IssuingDate { get; private set; }
        public string ExpiryDate { get; private set; }
        public string IssuingAuthority { get; private set; }
        public string DocumentSerialNumber { get; private set; }
        public string ChipSerialNumber { get; private set; }
        public string DocumentName { get; private set; }

        public static DocumentData CreateFromNativeData(EID_DOCUMENT_DATA nativeData)
        {
            try
            {
                return new DocumentData
                {
                    DocumentRegistrationNumber = Encoding.UTF8.GetString(nativeData.DocRegNo, 0, nativeData.DocRegNoSize),
                    DocumentType = Encoding.UTF8.GetString(nativeData.DocumentType, 0, nativeData.DocumentTypeSize),
                    IssuingDate = Encoding.UTF8.GetString(nativeData.IssuingDate, 0, nativeData.IssuingDateSize),
                    ExpiryDate = Encoding.UTF8.GetString(nativeData.ExpiryDate, 0, nativeData.ExpiryDateSize),
                    IssuingAuthority = Encoding.UTF8.GetString(nativeData.IssuingAuthority, 0, nativeData.IssuingAuthoritySize),
                    DocumentSerialNumber = Encoding.UTF8.GetString(nativeData.DocumentSerialNumber, 0, nativeData.DocumentSerialNumberSize),
                    ChipSerialNumber = Encoding.UTF8.GetString(nativeData.ChipSerialNumber, 0, nativeData.ChipSerialNumberSize),
                    DocumentName = Encoding.UTF8.GetString(nativeData.DocumentName, 0, nativeData.DocumentNameSize)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create DocumentData", ex);
            }
        }
    }
}