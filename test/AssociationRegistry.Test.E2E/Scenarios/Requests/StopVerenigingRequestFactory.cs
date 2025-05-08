namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Stop.RequestModels;
using Alba;
using Events;
using Framework.ApiSetup;
using Vereniging;
using FeitelijkeVereniging;
using Marten;
using System.Net;

public class StopVerenigingRequestFactory : ITestRequestFactory<StopVerenigingRequest>
{
    private readonly IFeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public StopVerenigingRequestFactory(IFeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<StopVerenigingRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new StopVerenigingRequest
        {
            Einddatum = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date)
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/stop");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<StopVerenigingRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request, sequence);
    }

    protected async Task WaitForAdresMatchEvent(FullBlownApiSetup apiSetup)
    {
        await using var session = apiSetup.AdminProjectionHost.DocumentStore().LightweightSession();
        var events = await session.Events.FetchStreamAsync(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);

        var counter = 0;

        while (!events.Any(a => a.EventType == typeof(AdresWerdOvergenomenUitAdressenregister)))
        {
            await Task.Delay(300);
            events = await session.Events.FetchStreamAsync(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);

            if (++counter > 20)
                throw new Exception(
                    $"Kept waiting for Adresmatch... Events committed: {string.Join(separator: ", ", events.Select(x => x.EventTypeName))}");
        }
    }}
