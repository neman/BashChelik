using BashChelik.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.DataModel
{
    public class DocumentData
    {
        public string DocumentRegistrationNumber { get; private set; }        

        public string DocumentType { get; private set; }

        public string IssuingDate { get; private set; }

        public string ExpiryDate { get; private set; }
        
        public string IssuingAuthority { get; private set; }        

        public DocumentData(EID_DOCUMENT_DATA nativeData)
        {
            DocumentRegistrationNumber = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.docRegNo));
            DocumentType = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.documentType));            
            IssuingDate = Encoding.UTF8.GetString(nativeData.issuingDate);
            ExpiryDate = Encoding.UTF8.GetString(nativeData.expiryDate);
            IssuingAuthority = Encoding.UTF8.GetString(Util.TrimTrailingZeroes(nativeData.issuingAuthority));
        }
    }
}
