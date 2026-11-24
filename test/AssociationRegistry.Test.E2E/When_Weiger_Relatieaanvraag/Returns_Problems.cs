namespace AssociationRegistry.Test.E2E.When_Weiger_Relatieaanvraag;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;
using Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;
using Alba;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using Resources;
using Xunit;

/// <summary>Requests that are refused after the refusal: they do not change the scenario.</summary>
[Collection(nameof(WeigerRelatieaanvraagCollection))]
public class Returns_Problems
{
    private readonly WeigerRelatieaanvraagContext _testContext;

    public Returns_Problems(WeigerRelatieaanvraagContext testContext)
    {
        _testContext = testContext;
    }

    private string Koepel => _testContext.Scenario.KoepelVCode;
    private string Vereniging => _testContext.Scenario.VerenigingVCode;

    [Fact]
    public async ValueTask Refusing_Again_Then_400() =>
        await Weiger(_testContext.AanvraagId, namens: Vereniging, ExceptionMessages.RelatieaanvraagWerdAlGeweigerd);

    [Fact]
    public async ValueTask Refusing_As_The_Aanvrager_Then_400() =>
        await Weiger(_testContext.AanvraagId, namens: Koepel, ExceptionMessages.NamensIsNietDeOntvanger);

    [Fact]
    public async ValueTask With_An_Invalid_AanvraagId_Then_400() =>
        await Weiger("V0001001", namens: Vereniging, new RelatieaanvraagIdIsOngeldig().Message);

    [Fact]
    public async ValueTask Confirming_After_The_Refusal_Then_400()
    {
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new BevestigRelatieaanvraagRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{_testContext.AanvraagId}/bevestiging");
            s.WithRequestHeader(WellknownHeaderNames.Namens, Vereniging);

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(ExceptionMessages.RelatieaanvraagWerdAlGeweigerd);
        });
    }

    [Fact]
    public async ValueTask Without_vr_namens_Then_400()
    {
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new WeigerRelatieaanvraagRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{_testContext.AanvraagId}/weigering");

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain($"{WellknownHeaderNames.Namens} is verplicht.");
        });
    }

    private async Task Weiger(string aanvraagId, string namens, string expectedMessage) =>
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new WeigerRelatieaanvraagRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{aanvraagId}/weigering");
            s.WithRequestHeader(WellknownHeaderNames.Namens, namens);

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(expectedMessage);
        });
}
