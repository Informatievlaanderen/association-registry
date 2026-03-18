namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record Contactgegeven : IHasBron
{
    public int ContactgegevenId { get; init; }
    public string Contactgegeventype { get; init; } = null!;
    public string Waarde { get; init; } = null!;
    public string? Beschrijving { get; init; }
    public bool IsPrimair { get; init; }
    public string Bron { get; set; } = null!;
}
