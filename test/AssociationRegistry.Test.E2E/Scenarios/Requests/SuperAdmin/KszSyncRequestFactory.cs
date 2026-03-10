namespace AssociationRegistry.Test.E2E.Scenarios.Requests.SuperAdmin;

using System.Net;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;

public class KszSyncRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly VzerWerdGeregistreerdForKszSyncScenario _scenario;
    private readonly string _insz;
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    public KszSyncRequestFactory(VzerWerdGeregistreerdForKszSyncScenario scenario, string insz)
    {
        _scenario = scenario;
        _insz = insz;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var vCode = _scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;
        var vertegenwoordigerId = _scenario.Vertegenwoordiger.VertegenwoordigerId;

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.WithRequestHeader(
                    "Authorization",
                    apiSetup.SuperAdminHttpClient.DefaultRequestHeaders.GetValues("Authorization").First()
                );
                s.Post.Url($"/v1/admin/sync/ksz/{_insz}");

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);

                // s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                // s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
            })
        ).Context.Response;

        // long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        await apiSetup.WaitForBewaartermijnEvent(vCode, vertegenwoordigerId);

        return new CommandResult<NullRequest>(VCode.Create(vCode), new NullRequest(), 1);
        //TODO change sequence? when implemting expected sequence?
    }
}
