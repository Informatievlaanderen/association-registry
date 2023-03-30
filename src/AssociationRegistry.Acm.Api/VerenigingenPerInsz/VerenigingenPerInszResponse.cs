namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using System.Collections.Immutable;
using System.Runtime.Serialization;

public record VerenigingenPerInszResponse(
    string Insz,
    [property: DataMember] ImmutableArray<Vereniging> Verenigingen);
