using System.Runtime.Serialization;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

[DataContract]
public record ListVerenigingContext(
    [property: DataMember(Name = "@base")] string Base,
    [property: DataMember(Name = "@vocab")]
    string Vocab,
    [property: DataMember(Name = "Identificator")]
    string Identificator,
    [property: DataMember(Name = "Id")] string Id,
    [property: DataMember(Name = "naam")]
    ContextType Naam
);

[DataContract]
public record struct ContextType(
    [property: DataMember(Name = "@id")] string Id,
    [property: DataMember(Name = "@type")] string Type
);
