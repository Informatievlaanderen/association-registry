namespace AssociationRegistry.JsonLdContext;

using Be.Vlaanderen.Basisregisters.Generators.Guid;
using System.Text;

public class JsonLdType
{
    public static readonly JsonLdType Vereniging = new(JsonLdContext.GuidNamespace.Vereniging, type: "fei:FeitelijkeVerenigingen");
    public static readonly JsonLdType Hoofdactiviteit = new(JsonLdContext.GuidNamespace.Hoofdactiviteit, type: "skos:Concept");

    public static readonly JsonLdType Contactgegeven =
        new(JsonLdContext.GuidNamespace.Contactgegeven, type: "schema:ContactPoint", prefix: "cont");

    public static readonly JsonLdType Locatie = new(JsonLdContext.GuidNamespace.Locatie, type: "org:Site", prefix: "loc");
    public static readonly JsonLdType Adres = new(JsonLdContext.GuidNamespace.Adres, type: "locn:Address", prefix: "adressen");
    public static readonly JsonLdType Sleutel = new(JsonLdContext.GuidNamespace.Sleutel, type: "adms:Identifier", prefix: "identificatoren");

    public static readonly JsonLdType GestructureerdeSleutel = new(JsonLdContext.GuidNamespace.GestructureerdeIdentificator,
                                                                   type: "generiek:GestructureerdeIdentificator", prefix: "ges");

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
