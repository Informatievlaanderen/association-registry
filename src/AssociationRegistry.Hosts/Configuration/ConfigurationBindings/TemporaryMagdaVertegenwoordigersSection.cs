namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

public class TemporaryMagdaVertegenwoordigersSection
{
    public const string SectionName = "TemporaryMagdaVertegenwoordigers";
    public TemporaryVertegenwoordiger[] TemporaryVertegenwoordigers { get; set; } = Array.Empty<TemporaryVertegenwoordiger>();

    public class TemporaryVertegenwoordiger
    {
        public string Insz { get; set; } = null!;
        public string Voornaam { get; set; } = null!;
        public string Achternaam { get; set; } = null!;
    }
}
