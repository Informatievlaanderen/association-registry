namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Verwijder.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using System.Net;
using Vereniging;

public class VerwijderVerenigingRequestFactory : ITestRequestFactory<VerwijderVerenigingRequest>
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public VerwijderVerenigingRequestFactory(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<VerwijderVerenigingRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new VerwijderVerenigingRequest()
        {
            Reden = "Foute vereniging",
        };


       var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {

            foreach (var defaultRequestHeader in apiSetup.SuperAdminHttpClient.DefaultRequestHeaders)
            {
                foreach (var value in defaultRequestHeader.Value)
                {
                    s.WithRequestHeader(defaultRequestHeader.Key, value);
                }
            }
            s.Delete
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens.VCode}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<VerwijderVerenigingRequest>(
            VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens.VCode), request, sequence);
    }
}
