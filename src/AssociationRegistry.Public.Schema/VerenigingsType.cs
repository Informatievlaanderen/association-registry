namespace AssociationRegistry.Public.Schema;

using Vereniging;
using Vereniging.Verenigingstype;

public record VerenigingsType : IVerenigingstype
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
