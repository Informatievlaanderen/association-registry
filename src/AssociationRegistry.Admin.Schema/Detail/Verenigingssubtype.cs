namespace AssociationRegistry.Admin.Schema.Detail;

using DecentraalBeheer.Vereniging;
using Vereniging;

public record Verenigingssubtype : IVerenigingssubtypeCode
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
