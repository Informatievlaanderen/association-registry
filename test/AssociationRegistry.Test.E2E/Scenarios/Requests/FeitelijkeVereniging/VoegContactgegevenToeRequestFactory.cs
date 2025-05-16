namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Marten.Events;
using System.Net;
using Vereniging;

public class VoegContactgegevenToeRequestFactory : ITestRequestFactory<VoegContactgegevenToeRequest>
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public VoegContactgegevenToeRequestFactory(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<VoegContactgegevenToeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegContactgegevenToeRequest>();

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/contactgegevens");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new CommandResult<VoegContactgegevenToeRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request);
    }
}
