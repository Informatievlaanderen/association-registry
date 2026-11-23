namespace AssociationRegistry.Test.E2E.Scenarios.Requests.Relatieaanvragen;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.Relatieaanvragen;

/// <summary>The koepel asks the vereniging to become lid: vr-namens is the koepel.</summary>
public class DienRelatieaanvraagInRequestFactory : ITestRequestFactory<DienRelatieaanvraagInRequest>
{
    private readonly KoepelEnVerenigingScenario _scenario;

    public DienRelatieaanvraagInRequestFactory(KoepelEnVerenigingScenario scenario)
    {
        _scenario = scenario;
    }

    public string AanvraagId { get; private set; } = null!;

    public async Task<CommandResult<DienRelatieaanvraagInRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = _scenario.KoepelVCode,
            VerenigingVCode = _scenario.VerenigingVCode,
            RelatieType = "Lidmaatschap",
            Beschrijving = "Aanvraag om lid te worden van de koepel",
        };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc).ToUrl("/v1/relatieaanvragen");
                s.WithRequestHeader(WellknownHeaderNames.Namens, _scenario.KoepelVCode);

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                s.Header("Location").ShouldHaveValues();
            })
        ).Context.Response;

        AanvraagId = response.Headers.Location.First()!.Split('/').Last();

        return new CommandResult<DienRelatieaanvraagInRequest>(
            VCode.Create(_scenario.KoepelVCode),
            request,
            Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First())
        );
    }
}
