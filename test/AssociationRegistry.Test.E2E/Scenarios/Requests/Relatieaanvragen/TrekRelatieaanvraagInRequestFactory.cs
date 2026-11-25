namespace AssociationRegistry.Test.E2E.Scenarios.Requests.Relatieaanvragen;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;

/// <summary>The aanvrager withdraws the relatieaanvraag, with a reden: vr-namens is the aanvrager.</summary>
public class TrekRelatieaanvraagInRequestFactory : ITestRequestFactory<TrekRelatieaanvraagInRequest>
{
    public const string Reden = "De aanvraag werd per vergissing ingediend.";

    private readonly string _aanvraagId;
    private readonly string _aanvrager;

    public TrekRelatieaanvraagInRequestFactory(string aanvraagId, string aanvrager)
    {
        _aanvraagId = aanvraagId;
        _aanvrager = aanvrager;
    }

    public async Task<CommandResult<TrekRelatieaanvraagInRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new TrekRelatieaanvraagInRequest { Reden = Reden };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc).ToUrl($"/v1/relatieaanvragen/{_aanvraagId}/intrekking");
                s.WithRequestHeader(WellknownHeaderNames.Namens, _aanvrager);

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            })
        ).Context.Response;

        return new CommandResult<TrekRelatieaanvraagInRequest>(
            VCode.Create(_aanvrager),
            request,
            Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First())
        );
    }
}
