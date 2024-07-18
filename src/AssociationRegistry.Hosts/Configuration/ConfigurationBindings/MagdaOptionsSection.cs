namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

public class MagdaOptionsSection
{
    public const string SectionName = "MagdaOptions";
    public string? ClientCertificate { get; set; }
    public string? ClientCertificatePassword { get; set; }
    public int Timeout { get; set; } = 30;
    public string Afzender { get; set; } = null!;
    public string Hoedanigheid { get; set; } = null!;
    public string Ontvanger { get; set; } = null!;
    public string? GeefOndernemingVkboEndpoint { get; set; } = null!;
    public string? GeefOndernemingEndpoint { get; set; } = null!;
    public string? RegistreerInschrijvingEndpoint { get; set; } = null!;
    public string? RegistreerUitschrijvingEndpoint { get; set; } = null!;
}
