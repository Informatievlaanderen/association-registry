namespace AssociationRegistry.Public.Schema.Constants;

using Be.Vlaanderen.Basisregisters.Generators.Guid;

public class JsonLdType
{
    public static readonly JsonLdType Vereniging = new(Constants.GuidNamespace.Vereniging, type: "fei:FeitelijkeVereniging");
    public Guid GuidNamespace { get; }
    public string Type { get; }
    public string Prefix { get; }

    private JsonLdType(Guid guidNamespace, string type, string prefix = "")
    {
        GuidNamespace = guidNamespace;
        Type = type;
        Prefix = prefix;
    }

    public string CreateWithIdValue(string value) => string.Join(separator: ':', Prefix, Deterministic.Create(GuidNamespace, value));
}
