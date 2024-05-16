using BashChelik.Native;
using System;
using System.Globalization;
using System.Text;

namespace BashChelik.DataModel
{
    public class Document
    {
        private Document()
        { }

        private string _issuingDate;
        private string _expiryDate;

        public string Name { get; private set; }
        public string RegistrationNumber { get; private set; }
        public string Type { get; private set; }
        public DateTime IssuingDate => DateTime.Parse(_issuingDate, CultureInfo.GetCultureInfo("sr-RS")).Date;
        public DateTime ExpiryDate => DateTime.Parse(_expiryDate, CultureInfo.GetCultureInfo("sr-RS")).Date;

        public string IssuingAuthority { get; private set; }
        public string SerialNumber { get; private set; }
        public string ChipSerialNumber { get; private set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{nameof(RegistrationNumber)}: {RegistrationNumber}");
            stringBuilder.AppendLine($"{nameof(Type)}: {Type}");
            stringBuilder.AppendLine($"{nameof(IssuingDate)}: {IssuingDate.ToShortDateString()}");
            stringBuilder.AppendLine($"{nameof(ExpiryDate)}: {ExpiryDate.ToShortDateString()}");
            stringBuilder.AppendLine($"{nameof(IssuingAuthority)}: {IssuingAuthority}");
            stringBuilder.AppendLine($"{nameof(SerialNumber)}: {SerialNumber}");
            stringBuilder.AppendLine($"{nameof(ChipSerialNumber)}: {ChipSerialNumber}");
            stringBuilder.AppendLine($"{nameof(Name)}: {Name}");

            return stringBuilder.ToString();
        }

        internal static Document CreateFromNativeData(EidDocumentData nativeData)
        {
            try
            {
                return new Document
                {
                    RegistrationNumber = Encoding.UTF8.GetString(nativeData.DocRegNo, 0, nativeData.DocRegNoSize),
                    Type = Encoding.UTF8.GetString(nativeData.DocumentType, 0, nativeData.DocumentTypeSize),
                    _issuingDate = Encoding.UTF8.GetString(nativeData.IssuingDate, 0, nativeData.IssuingDateSize),
                    _expiryDate = Encoding.UTF8.GetString(nativeData.ExpiryDate, 0, nativeData.ExpiryDateSize),
                    IssuingAuthority = Encoding.UTF8.GetString(nativeData.IssuingAuthority, 0, nativeData.IssuingAuthoritySize),
                    SerialNumber = Encoding.UTF8.GetString(nativeData.DocumentSerialNumber, 0, nativeData.DocumentSerialNumberSize),
                    ChipSerialNumber = Encoding.UTF8.GetString(nativeData.ChipSerialNumber, 0, nativeData.ChipSerialNumberSize),
                    Name = Encoding.UTF8.GetString(nativeData.DocumentName, 0, nativeData.DocumentNameSize)
                };
            }
            catch (Exception ex)
            {
                throw new BashChelikException($"Failed to create {nameof(Document)}", ex);
            }
        }
    }
}