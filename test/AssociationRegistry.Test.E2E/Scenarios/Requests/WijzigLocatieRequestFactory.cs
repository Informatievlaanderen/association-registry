namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using FeitelijkeVereniging;
using Framework.ApiSetup;
using System.Net;
using Vereniging;
using Adres = Admin.Api.WebApi.Verenigingen.Common.Adres;

public class WijzigLocatieRequestFactory : ITestRequestFactory<WijzigLocatieRequest>
{
    private readonly IFeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public WijzigLocatieRequestFactory(IFeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<WijzigLocatieRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigLocatieRequest()
        {
            Locatie =
                new TeWijzigenLocatie
                {
                    Naam = "Kantoor",
                    Adres = new Adres
                    {
                        Straatnaam = "Leopold II-laan",
                        Huisnummer = "99",
                        Busnummer = "",
                        Postcode = "9200",
                        Gemeente = "Dendermonde",
                        Land = "België",
                    },
                    IsPrimair = true,
                    Locatietype = Locatietype.Correspondentie,
                },
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl(
                  $"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/locaties/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0].LocatieId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        var newSequence = await apiSetup.WaitForAdresMatchEventForEachLocation(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode, 1);

        return new CommandResult<WijzigLocatieRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request,
                                                       newSequence);
    }
}
