namespace AssociationRegistry.Admin.Schema;

using Vereniging;


public record VerenigingsType : IVerenigingstype
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
