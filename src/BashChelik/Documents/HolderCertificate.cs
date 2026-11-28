using System.Security.Cryptography.X509Certificates;

namespace BashChelik;

/// <summary>A certificate the Ministry of Interior issued to the holder, stored on the document.</summary>
public sealed class HolderCertificate
{
    /// <summary>Creates a holder certificate.</summary>
    public HolderCertificate(CertificatePurpose purpose, X509Certificate2 certificate)
    {
        Purpose = purpose;
        Certificate = certificate;
    }

    /// <summary>What the certificate is for.</summary>
    public CertificatePurpose Purpose { get; }

    /// <summary>The certificate.</summary>
    public X509Certificate2 Certificate { get; }
}
