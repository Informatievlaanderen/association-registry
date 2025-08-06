namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Subtype.Examples;

using AssociationRegistry.Vereniging;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigSubtypeRequestExamples : IMultipleExamplesProvider<WijzigSubtypeRequest>
{
    // TODO:
    public IEnumerable<SwaggerExample<WijzigSubtypeRequest>> GetExamples()
    {
        yield return new SwaggerExample<WijzigSubtypeRequest>
        {
            Name = "Verfijn subtype naar feitelijke vereniging.",
            Summary = "Verfijn subtype naar feitelijke vereniging.",
            Value = new WijzigSubtypeRequest
            {
                Subtype = VerenigingssubtypeCode.FeitelijkeVereniging.Code,
                Beschrijving = null,
                Identificatie = null,
                AndereVereniging = null,
            },
        };

        yield return new SwaggerExample<WijzigSubtypeRequest>
        {
            Name = "Zet subtype terug naar niet bepaald.",
            Summary = "Zet subtype terug naar niet bepaald.",
            Value = new WijzigSubtypeRequest
            {
                Subtype = VerenigingssubtypeCode.NietBepaald.Code,
                Beschrijving = null,
                Identificatie = null,
                AndereVereniging = null,
            },
        };

        yield return new SwaggerExample<WijzigSubtypeRequest>
        {
            Name = "Verfijn subtype naar subvereniging.",
            Summary = "Verfijn subtype naar subvereniging.",
            Value = new WijzigSubtypeRequest
            {
                Subtype = VerenigingssubtypeCode.Subvereniging.Code,
                AndereVereniging = "V0001002",
                Beschrijving = "De subvereniging van V0001002",
                Identificatie = "0012"
            },
        };

        yield return new SwaggerExample<WijzigSubtypeRequest>
        {
            Name = "Wijzig het subtype subvereniging.",
            Summary = "Wijzig het subtype subvereniging.",
            Value = new WijzigSubtypeRequest
            {
                Subtype = VerenigingssubtypeCode.Subvereniging.Code,
                AndereVereniging = "V0001002",
                Beschrijving = "De subvereniging van V0001002",
                Identificatie = "0012"
            },
        };
    }
}
