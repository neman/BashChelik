namespace BashChelik.DataModel
{
    public class ElectronicIdentificationData
    {
        public CardType CardType { get; set; }
        public Document Document { get; set; }
        public FixedPersonalData FixedPersonalData { get; set; }
        public VariablePersonalData VariablePersonalData { get; set; }
        public PortraitData PortraitData { get; set; }
        public CertificateData CertificateData { get; set; }
    }
}