using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BashChelik.DataModel
{
    public class ElectronicIdentificationData
    {
        public DocumentData DocumentData { get; set; }
        public FixedPersonalData FixedPersonalData { get; set; }
        public VariablePersonalData VariablePersonalData { get; set; }
        public PortraitData PortraitData { get; set; }
        public CertificateData CertificateData { get; set; }
    }
}
