namespace AssociationRegistry.Integrations.Magda;

using Extensions;
using Models;
using Models.GeefOnderneming;
using Onderneming.GeefOnderneming;

public static class MagdaResponseValidator
{
    public static bool HasBlokkerendeUitzonderingen(ResponseEnvelope<GeefOndernemingResponseBody>? magdaResponse)
        => magdaResponse.HasUitzonderingenOfType(UitzonderingTypeType.FOUT) ||
           magdaResponse.HasAnyUitzondering(x => MagdaUitzondering.IsGekendBlokkerendeUitzondering(x.Identificatie));
}
