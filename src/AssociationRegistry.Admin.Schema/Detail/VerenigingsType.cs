namespace AssociationRegistry.Admin.Schema.Detail;

public record VerenigingsType
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}