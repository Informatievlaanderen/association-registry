using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace AssociationRegistry.Public.Api.SearchVerenigingen;

[DataContract]
public record SearchVerenigingenResponse(
    [property:DataMember(Name = "Verenigingen")]ImmutableArray<Vereniging> Verenigingen);
