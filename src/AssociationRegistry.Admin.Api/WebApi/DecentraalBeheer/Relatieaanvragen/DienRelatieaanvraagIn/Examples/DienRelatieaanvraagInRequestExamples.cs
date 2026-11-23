namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using DomainRelatieType = DecentraalBeheer.Relatieaanvragen.RelatieType;

public class DienRelatieaanvraagInRequestExamples : IExamplesProvider<DienRelatieaanvraagInRequest>
{
    public DienRelatieaanvraagInRequest GetExamples() =>
        new()
        {
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001002",
            RelatieType = DomainRelatieType.Lidmaatschap.Value,
            Beschrijving = "Aanvraag lidmaatschap bij de koepel",
        };
}
