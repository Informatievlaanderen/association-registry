namespace AssociationRegistry.Admin.Schema.Detail;

using DecentraalBeheer.Vereniging;
using Vereniging;

public record Verenigingstype : IVerenigingstype
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
