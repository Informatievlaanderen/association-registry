namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Alba;
using Events;
using Framework.ApiSetup;
using Vereniging;
using FeitelijkeVereniging;
using Marten;
using Marten.Events;
using System.Net;
using Adres = Admin.Api.Verenigingen.Common.Adres;

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
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/locaties/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0].LocatieId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        await WaitForAdresMatchEvent(apiSetup);

        return new CommandResult<WijzigLocatieRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request, sequence);
    }

    protected async Task WaitForAdresMatchEvent(IApiSetup apiSetup)
    {
        await using var session = apiSetup.AdminProjectionHost.DocumentStore().LightweightSession();
        var events = await session.Events.FetchStreamAsync(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);

        var counter = 0;

        while (!events.Any(a => a.EventType == typeof(AdresWerdOvergenomenUitAdressenregister)))
        {
            await Task.Delay(200);
            events = await session.Events.FetchStreamAsync(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);

            if (++counter > 50)
                throw new Exception(
                    $"Kept waiting for Adresmatch... Events committed: {string.Join(separator: ", ", events.Select(x => x.EventTypeName))}");
        }
    }}
