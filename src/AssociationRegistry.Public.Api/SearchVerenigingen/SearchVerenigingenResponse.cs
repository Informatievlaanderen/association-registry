using System.Collections.Immutable;

namespace AssociationRegistry.Public.Api.VerenigingenPerRijksregisternummer;

public record SearchVerenigingenResponse(ImmutableArray<Vereniging> Verenigingen);
