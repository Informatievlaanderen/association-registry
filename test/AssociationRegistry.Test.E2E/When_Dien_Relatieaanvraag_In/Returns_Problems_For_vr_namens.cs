namespace AssociationRegistry.Test.E2E.When_Dien_Relatieaanvraag_In;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;
using Alba;
using Resources;
using Xunit;

/// <summary>Requests that are refused before anything is saved: they do not change the scenario.</summary>
[Collection(nameof(DienRelatieaanvraagInCollection))]
public class Returns_Problems_For_vr_namens
{
    private readonly DienRelatieaanvraagInContext _testContext;

    public Returns_Problems_For_vr_namens(DienRelatieaanvraagInContext testContext)
    {
        _testContext = testContext;
    }

    private DienRelatieaanvraagInRequest Request =>
        new()
        {
            KoepelVCode = _testContext.Scenario.KoepelVCode,
            VerenigingVCode = _testContext.Scenario.VerenigingVCode,
            RelatieType = "Subvereniging",
        };

    [Fact]
    public async ValueTask Without_vr_namens_Then_400()
    {
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(Request, JsonStyle.Mvc).ToUrl("/v1/relatieaanvragen");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain($"{WellknownHeaderNames.Namens} is verplicht.");
        });
    }

    [Fact]
    public async ValueTask With_A_vr_namens_That_Is_No_Partij_Then_400()
    {
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(Request, JsonStyle.Mvc).ToUrl("/v1/relatieaanvragen");
            s.WithRequestHeader(WellknownHeaderNames.Namens, "V0009999");
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(ExceptionMessages.AanvragerIsGeenPartijBijRelatieaanvraag);
        });
    }
}
