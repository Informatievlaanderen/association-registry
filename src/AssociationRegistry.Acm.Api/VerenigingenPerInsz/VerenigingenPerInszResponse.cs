namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record VerenigingenPerInszResponse(
    [property: DataMember] string Insz,
    [property: DataMember] ImmutableArray<Vereniging> Verenigingen);
