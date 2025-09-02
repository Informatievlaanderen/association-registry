namespace AssociationRegistry.Grar.NutsLau;

using Marten.Schema;

public record PostalNutsLauInfo
{
    [Identity]
    public string Postcode { get; set; }
    public string Gemeentenaam { get; set; }
    public string Nuts2 => Nuts3[..4];
    public string Nuts3 { get; set; }
    public string Lau { get; set; }
    public string Nuts3Lau => $"{Nuts3}{Lau}";
    public string SomethingElse { get; set; }
};
