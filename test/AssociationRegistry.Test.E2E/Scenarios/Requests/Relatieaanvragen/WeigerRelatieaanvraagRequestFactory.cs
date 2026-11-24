namespace AssociationRegistry.Test.E2E.Scenarios.Requests.Relatieaanvragen;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;

/// <summary>The ontvanger refuses the relatieaanvraag, with a reden: vr-namens is the ontvanger.</summary>
public class WeigerRelatieaanvraagRequestFactory : ITestRequestFactory<WeigerRelatieaanvraagRequest>
{
    public const string Reden = "Wij zijn geen lid van deze koepel.";

    private readonly string _aanvraagId;
    private readonly string _ontvanger;

    public WeigerRelatieaanvraagRequestFactory(string aanvraagId, string ontvanger)
    {
        _aanvraagId = aanvraagId;
        _ontvanger = ontvanger;
    }

    public async Task<CommandResult<WeigerRelatieaanvraagRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WeigerRelatieaanvraagRequest { Reden = Reden };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc).ToUrl($"/v1/relatieaanvragen/{_aanvraagId}/weigering");
                s.WithRequestHeader(WellknownHeaderNames.Namens, _ontvanger);

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            })
        ).Context.Response;

        return new CommandResult<WeigerRelatieaanvraagRequest>(
            VCode.Create(_ontvanger),
            request,
            Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First())
        );
    }
}
