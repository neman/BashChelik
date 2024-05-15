using BashChelik.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.DataModel
{
    public class CertificateData
    {
        public byte[] Certificate { get; set; }

        public CertificateData(EID_CERTIFICATE nativeData)
        {
            Certificate = nativeData.certificate;
        }
    }
}
