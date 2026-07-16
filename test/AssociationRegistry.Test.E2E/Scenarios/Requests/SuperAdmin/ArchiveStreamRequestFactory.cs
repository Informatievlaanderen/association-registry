namespace AssociationRegistry.Test.E2E.Scenarios.Requests.SuperAdmin;

using System.Net;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Vereniging;

public class ArchiveStreamRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public ArchiveStreamRequestFactory(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var vCode = _scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            foreach (var defaultRequestHeader in apiSetup.SuperAdminHttpClient.DefaultRequestHeaders)
            {
                foreach (var value in defaultRequestHeader.Value)
                {
                    s.WithRequestHeader(defaultRequestHeader.Key, value);
                }
            }

            s.Delete.Url($"/v1/maintenance/streams/{vCode}");

            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        return new CommandResult<NullRequest>(VCode.Create(vCode), new NullRequest());
    }
}
