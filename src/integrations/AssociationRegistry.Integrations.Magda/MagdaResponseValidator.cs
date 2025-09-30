namespace AssociationRegistry.Integrations.Magda;

using Extensions;
using GeefOnderneming.Models;
using Models;
using Onderneming.GeefOnderneming;

public static class MagdaResponseValidator
{
    public static bool HasBlokkerendeUitzonderingen(ResponseEnvelope<GeefOndernemingResponseBody>? magdaResponse)
        => magdaResponse.HasUitzonderingenOfType(UitzonderingTypeType.FOUT) ||
           magdaResponse.HasAnyUitzondering(x => MagdaUitzondering.IsGekendBlokkerendeUitzondering(x.Identificatie));
}
