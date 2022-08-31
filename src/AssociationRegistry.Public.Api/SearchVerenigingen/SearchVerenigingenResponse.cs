using System.Collections.Immutable;

namespace AssociationRegistry.Public.Api.SearchVerenigingen;

public record SearchVerenigingenResponse(ImmutableArray<Vereniging> Verenigingen);
