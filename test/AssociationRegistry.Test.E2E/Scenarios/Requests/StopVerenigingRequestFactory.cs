namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Alba;
using Events;
using Framework.ApiSetup;
using Vereniging;
using FeitelijkeVereniging;
using Marten;
using Marten.Events;
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

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/stop");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new CommandResult<StopVerenigingRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request);
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
