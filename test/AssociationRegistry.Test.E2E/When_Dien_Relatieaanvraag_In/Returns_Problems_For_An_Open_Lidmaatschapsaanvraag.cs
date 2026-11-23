namespace AssociationRegistry.Test.E2E.When_Dien_Relatieaanvraag_In;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;
using Alba;
using Resources;
using Xunit;

/// <summary>
///     The scenario leaves an open lidmaatschapsaanvraag between the koepel and the vereniging: the vereniging cannot
///     ask to become subvereniging of another koepel (K6). The request is refused, so the scenario does not change.
/// </summary>
[Collection(nameof(DienRelatieaanvraagInCollection))]
public class Returns_Problems_For_An_Open_Lidmaatschapsaanvraag
{
    private readonly DienRelatieaanvraagInContext _testContext;

    public Returns_Problems_For_An_Open_Lidmaatschapsaanvraag(DienRelatieaanvraagInContext testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public async ValueTask With_A_Subvereniging_Of_Another_Koepel_Then_400()
    {
        var request = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = _testContext.Scenario.AndereKoepelVCode,
            VerenigingVCode = _testContext.Scenario.VerenigingVCode,
            RelatieType = "Subvereniging",
        };

        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(request, JsonStyle.Mvc).ToUrl("/v1/relatieaanvragen");
            s.WithRequestHeader(WellknownHeaderNames.Namens, _testContext.Scenario.VerenigingVCode);
            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(
                string.Format(
                    ExceptionMessages.VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden,
                    _testContext.AanvraagId
                )
            );
        });
    }
}
