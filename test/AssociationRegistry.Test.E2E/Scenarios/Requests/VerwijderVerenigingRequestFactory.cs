namespace AssociationRegistry.Test.E2E.Scenarios.Requests;

using Admin.Api.Verenigingen.Verwijder.RequestModels;
using Alba;
using Be.Vlaanderen.Basisregisters.Utilities;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Marten.Events;
using System.Net;
using Vereniging;

public class VerwijderVerenigingRequestFactory : ITestRequestFactory<VerwijderVerenigingRequest>
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public VerwijderVerenigingRequestFactory(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<RequestResult<VerwijderVerenigingRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new VerwijderVerenigingRequest()
        {
            Reden = "Foute vereniging",
        };


        await apiSetup.AdminApiHost.Scenario(s =>
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
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<VerwijderVerenigingRequest>(
            VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request);
    }
}
