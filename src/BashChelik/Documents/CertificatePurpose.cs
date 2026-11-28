namespace BashChelik;

/// <summary>What a holder certificate is for.</summary>
public enum CertificatePurpose
{
    /// <summary>Identifying the holder, for example when signing in.</summary>
    Authentication,

    /// <summary>Signing documents in the holder's name.</summary>
    Signing,
}
