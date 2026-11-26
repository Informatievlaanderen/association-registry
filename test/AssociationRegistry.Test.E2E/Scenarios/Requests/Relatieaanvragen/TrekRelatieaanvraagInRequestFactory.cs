namespace AssociationRegistry.Test.E2E.Scenarios.Requests.Relatieaanvragen;

using System.Net;
using Admin.Api.Infrastructure;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;

/// <summary>The aanvrager withdraws the relatieaanvraag, without a body: vr-namens is the aanvrager.</summary>
public class TrekRelatieaanvraagInRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly string _aanvraagId;
    private readonly string _aanvrager;

    public TrekRelatieaanvraagInRequestFactory(string aanvraagId, string aanvrager)
    {
        _aanvraagId = aanvraagId;
        _aanvrager = aanvrager;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Url($"/v1/relatieaanvragen/{_aanvraagId}/intrekking");
                s.WithRequestHeader(WellknownHeaderNames.Namens, _aanvrager);

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            })
        ).Context.Response;

        return new CommandResult<NullRequest>(
            VCode.Create(_aanvrager),
            new NullRequest(),
            Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First())
        );
    }
}
