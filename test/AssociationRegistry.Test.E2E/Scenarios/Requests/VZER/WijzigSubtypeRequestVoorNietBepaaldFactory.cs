namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Subtype.RequestModels;
using Alba;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using System.Net;

public class WijzigSubtypeRequestVoorNietBepaaldFactory : ITestRequestFactory<WijzigSubtypeRequest>
{
    private readonly SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario _scenario;

    public WijzigSubtypeRequestVoorNietBepaaldFactory(SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<WijzigSubtypeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigSubtypeRequest
        {
            Subtype = VerenigingssubtypeCode.NietBepaald.Code,
        };

       var response =  (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging.VCode}/subtype");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigSubtypeRequest>(VCode.Create(_scenario.VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging.VCode), request, sequence);
    }
}
