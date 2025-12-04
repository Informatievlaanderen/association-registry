namespace AssociationRegistry.Integrations.Magda.Persoon.Validation;

using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Integrations.Magda.Onderneming.Models.GeefOnderneming;
using AssociationRegistry.Integrations.Magda.Shared.Extensions;
using AssociationRegistry.Integrations.Magda.Shared.Models;

public static class MagdaResponseValidator
{
    public static bool HasBlokkerendeUitzonderingen(ResponseEnvelope<GeefOndernemingResponseBody>? magdaResponse)
        => magdaResponse.HasUitzonderingenOfType(UitzonderingTypeType.FOUT) ||
           magdaResponse.HasAnyUitzondering(x => MagdaUitzondering.IsGekendBlokkerendeUitzondering(x.Identificatie));
}
