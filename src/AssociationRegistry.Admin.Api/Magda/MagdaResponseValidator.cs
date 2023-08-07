namespace AssociationRegistry.Admin.Api.Magda;

using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;

public static class MagdaResponseValidator
{
    public static bool HasBlokkerendeUitzonderingen(ResponseEnvelope<GeefOndernemingResponseBody>? magdaResponse)
        => magdaResponse.HasUitzonderingenOfType(UitzonderingTypeType.FOUT) ||
           magdaResponse.HasAnyUitzondering(x => MagdaUitzondering.IsGekendBlokkerendeUitzondering(x.Identificatie));
}
