namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using System.Net;
using Vereniging;

public class VerwijderLidmaatschapRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly LidmaatschapWerdToegevoegdScenario _scenario;

    public VerwijderLidmaatschapRequestFactory(LidmaatschapWerdToegevoegdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var vCode = _scenario.LidmaatschapWerdToegevoegd.VCode;
        var lidmaatschapLidmaatschapId = _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId;

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Delete
             .Url($"/v1/verenigingen/{vCode}/lidmaatschappen/{lidmaatschapLidmaatschapId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<NullRequest>(VCode.Create(vCode), new NullRequest(), sequence);
    }
}
