namespace AssociationRegistry.Admin.Schema;

public record VerenigingsType : IVerenigingsType
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}


public interface IVerenigingsType
{
    string Code { get; init; }
    string Naam { get; init; }
}
