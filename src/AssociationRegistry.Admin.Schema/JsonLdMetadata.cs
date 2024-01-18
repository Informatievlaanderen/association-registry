namespace AssociationRegistry.Admin.Schema;

public class JsonLdMetadata
{
    public JsonLdMetadata()
    {
    }

    public JsonLdMetadata(string id, string type)
    {
        Id = id;
        Type = type;
    }

    public string Id { get; set; }
    public string Type { get; set; }
}
