namespace AssociationRegistry.Test.E2E.When_Bevestig_Relatieaanvraag;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;
using Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;
using Alba;
using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using Resources;
using Xunit;

/// <summary>Requests that are refused after the confirmation: they do not change the scenario.</summary>
[Collection(nameof(BevestigRelatieaanvraagCollection))]
public class Returns_Problems
{
    private readonly BevestigRelatieaanvraagContext _testContext;

    public Returns_Problems(BevestigRelatieaanvraagContext testContext)
    {
        _testContext = testContext;
    }

    private string Koepel => _testContext.Scenario.KoepelVCode;
    private string Vereniging => _testContext.Scenario.VerenigingVCode;

    [Fact]
    public async ValueTask Confirming_Again_Then_400() =>
        await Bevestig(_testContext.AanvraagId, namens: Vereniging, ExceptionMessages.RelatieaanvraagWerdAlBevestigd);

    [Fact]
    public async ValueTask Confirming_As_The_Aanvrager_Then_400() =>
        await Bevestig(_testContext.AanvraagId, namens: Koepel, ExceptionMessages.NamensIsNietDeOntvanger);

    [Fact]
    public async ValueTask With_An_Invalid_AanvraagId_Then_400() =>
        await Bevestig("V0001001", namens: Vereniging, new RelatieaanvraagIdIsOngeldig().Message);

    [Fact]
    public async ValueTask Without_vr_namens_Then_400()
    {
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new BevestigRelatieaanvraagRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{_testContext.AanvraagId}/bevestiging");

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain($"{WellknownHeaderNames.Namens} is verplicht.");
        });
    }

    /// <summary>Not "staat al open": the confirmation released the pair, and now the lidmaatschap blocks it.</summary>
    [Fact]
    public async ValueTask A_New_Relatieaanvraag_For_The_Same_Pair_Then_400_Because_Of_The_Lidmaatschap()
    {
        var request = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = Koepel,
            VerenigingVCode = Vereniging,
            RelatieType = RelatieType.Labels.Lidmaatschap,
        };

        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(request, JsonStyle.Mvc).ToUrl("/v1/relatieaanvragen");
            s.WithRequestHeader(WellknownHeaderNames.Namens, Koepel);

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(ExceptionMessages.ErBestaatAlEenActieveFeitelijkeRelatie);
        });
    }

    private async Task Bevestig(string aanvraagId, string namens, string expectedMessage) =>
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new BevestigRelatieaanvraagRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{aanvraagId}/bevestiging");
            s.WithRequestHeader(WellknownHeaderNames.Namens, namens);

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(expectedMessage);
        });
}
