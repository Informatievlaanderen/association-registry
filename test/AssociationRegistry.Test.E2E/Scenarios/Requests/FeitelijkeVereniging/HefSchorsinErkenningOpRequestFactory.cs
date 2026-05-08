namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using System.Net;
using Admin.Api.Infrastructure;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;

public class HefSchorsingErkenningOpRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly ErkenningWerdGeschorstScenario _scenario;

    public HefSchorsingErkenningOpRequestFactory(ErkenningWerdGeschorstScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var vCode = _scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.WithRequestHeader(
                    WellknownHeaderNames.Initiator,
                    _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
                );

                s.Delete.Url(
                    $"/v1/verenigingen/{vCode}/erkenningen/{_scenario.ErkenningWerdGeregistreerd.ErkenningId}/schorsingen"
                );

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<NullRequest>(VCode.Create(vCode), new NullRequest(), sequence);
    }
}
