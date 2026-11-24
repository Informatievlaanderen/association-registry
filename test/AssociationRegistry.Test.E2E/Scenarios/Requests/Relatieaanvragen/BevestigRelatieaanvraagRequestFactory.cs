namespace AssociationRegistry.Test.E2E.Scenarios.Requests.Relatieaanvragen;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;

/// <summary>The ontvanger confirms the relatieaanvraag: vr-namens is the ontvanger.</summary>
public class BevestigRelatieaanvraagRequestFactory : ITestRequestFactory<BevestigRelatieaanvraagRequest>
{
    private readonly string _aanvraagId;
    private readonly string _ontvanger;

    public BevestigRelatieaanvraagRequestFactory(string aanvraagId, string ontvanger)
    {
        _aanvraagId = aanvraagId;
        _ontvanger = ontvanger;
    }

    public async Task<CommandResult<BevestigRelatieaanvraagRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new BevestigRelatieaanvraagRequest
        {
            Identificatie = "LID-1",
            Beschrijving = "Lid van de koepel",
        };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc).ToUrl($"/v1/relatieaanvragen/{_aanvraagId}/bevestiging");
                s.WithRequestHeader(WellknownHeaderNames.Namens, _ontvanger);

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            })
        ).Context.Response;

        return new CommandResult<BevestigRelatieaanvraagRequest>(
            VCode.Create(_ontvanger),
            request,
            Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First())
        );
    }
}
