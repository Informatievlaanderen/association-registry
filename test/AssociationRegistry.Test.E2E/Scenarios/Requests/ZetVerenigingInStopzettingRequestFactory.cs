namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.InStopzetting.RequestModels;
using Admin.Api.WebApi.Verenigingen.Stop.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using Events;
using FeitelijkeVereniging;
using Framework.ApiSetup;
using Marten;
using Vereniging;

public class ZetVerenigingInStopzettingRequestFactory : ITestRequestFactory<InStopzettingRequest>
{
    private readonly IFeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public ZetVerenigingInStopzettingRequestFactory(IFeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<InStopzettingRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new InStopzettingRequest { InStopzetting = true };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc)
                    .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/in-stopzetting");

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<InStopzettingRequest>(
            VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode),
            request,
            sequence
        );
    }
}
