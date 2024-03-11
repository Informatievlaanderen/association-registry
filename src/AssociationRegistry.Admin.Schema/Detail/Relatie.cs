namespace AssociationRegistry.Admin.Schema.Detail;

public record Relatie
{
    public string Relatietype { get; init; } = null!;
    public GerelateerdeVereniging AndereVereniging { get; init; } = null!;

    public record GerelateerdeVereniging
    {
        public string KboNummer { get; init; } = null!;
        public string VCode { get; init; } = null!;
        public string Naam { get; init; } = null!;
    }
}