namespace AssociationRegistry.Test.E2E.Scenarios.Requests.Relatieaanvragen;

using System.Net;
using Admin.Api.Infrastructure;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;

/// <summary>The ontvanger refuses the relatieaanvraag, without a body: vr-namens is the ontvanger.</summary>
public class WeigerRelatieaanvraagRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly string _aanvraagId;
    private readonly string _ontvanger;

    public WeigerRelatieaanvraagRequestFactory(string aanvraagId, string ontvanger)
    {
        _aanvraagId = aanvraagId;
        _ontvanger = ontvanger;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Url($"/v1/relatieaanvragen/{_aanvraagId}/weigering");
                s.WithRequestHeader(WellknownHeaderNames.Namens, _ontvanger);

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            })
        ).Context.Response;

        return new CommandResult<NullRequest>(
            VCode.Create(_ontvanger),
            new NullRequest(),
            Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First())
        );
    }
}
