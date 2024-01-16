namespace AssociationRegistry.Public.Schema.Constants;

using Be.Vlaanderen.Basisregisters.Generators.Guid;
using System.Text;

public class JsonLdType
{
    public static readonly JsonLdType Vereniging = new(Constants.GuidNamespace.Vereniging, type: "fei:FeitelijkeVerenigingen");
    public static readonly JsonLdType Hoofdactiviteit = new(Constants.GuidNamespace.Hoofdactiviteit, type: "skos:Concept");

    public static readonly JsonLdType Contactgegeven =
        new(Constants.GuidNamespace.Contactgegeven, type: "schema:ContactPoint", prefix: "cont");

    public static readonly JsonLdType Locatie = new(Constants.GuidNamespace.Locatie, type: "org:Site", prefix: "loc");
    public static readonly JsonLdType Adres = new(Constants.GuidNamespace.Locatie, type: "locn:Address", prefix: "adressen");
    public Guid GuidNamespace { get; }
    public string Type { get; }
    public string Prefix { get; }

    private JsonLdType(Guid guidNamespace, string type, string prefix = "")
    {
        GuidNamespace = guidNamespace;
        Type = type;
        Prefix = prefix;
    }

    public string CreateWithIdValues(params string[] values)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(Prefix))
            sb.Append($"{Prefix}:");

        sb.Append(Deterministic.Create(GuidNamespace, string.Join(separator: '-', values)));

        return sb.ToString();
    }
}
