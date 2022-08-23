namespace AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;

using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record GetVerenigingenResponse(
    [property: DataMember] string Rijksregisternummer,
    [property: DataMember] ImmutableArray<Vereniging> Verenigingen);
