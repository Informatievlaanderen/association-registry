namespace AssociationRegistry.Admin.Schema.Detail;

using Vereniging;

public record Verenigingssubtype : IVerenigingssubtype
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
