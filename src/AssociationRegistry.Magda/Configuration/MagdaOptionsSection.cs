namespace AssociationRegistry.Magda.Configuration;

public class MagdaOptionsSection
{
    public const string SectionName = "MagdaOptions";

    public string ClientCertificate { get; set; } = null!;
    public string ClientCertificatePassword { get; set; } = null!;
    public int Timeout { get; set; } = 30;
    public string Afzender { get; set; } = null!;
    public string Hoedanigheid { get; set; } = null!;
    public string Ontvanger { get; set; } = null!;
    public string GeefOndernemingVkboEndpoint { get; set; } = null!;
}
