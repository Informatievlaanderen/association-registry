namespace AssociationRegistry.Test.E2E.When_Trek_Relatieaanvraag_In;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;
using Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn.RequestModels;
using Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;
using Alba;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using Resources;
using Xunit;

/// <summary>Requests that are refused after the intrekking: they do not change the scenario.</summary>
[Collection(nameof(TrekRelatieaanvraagInCollection))]
public class Returns_Problems
{
    private readonly TrekRelatieaanvraagInContext _testContext;

    public Returns_Problems(TrekRelatieaanvraagInContext testContext)
    {
        _testContext = testContext;
    }

    private string Koepel => _testContext.Scenario.KoepelVCode;
    private string Vereniging => _testContext.Scenario.VerenigingVCode;

    [Fact]
    public async ValueTask Withdrawing_Again_Then_400() =>
        await TrekIn(_testContext.AanvraagId, namens: Koepel, ExceptionMessages.RelatieaanvraagWerdAlIngetrokken);

    [Fact]
    public async ValueTask Withdrawing_As_The_Ontvanger_Then_400() =>
        await TrekIn(_testContext.AanvraagId, namens: Vereniging, ExceptionMessages.NamensIsNietDeAanvrager);

    [Fact]
    public async ValueTask With_An_Invalid_AanvraagId_Then_400() =>
        await TrekIn("V0001001", namens: Koepel, new RelatieaanvraagIdIsOngeldig().Message);

    [Fact]
    public async ValueTask Confirming_After_The_Intrekking_Then_400()
    {
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new BevestigRelatieaanvraagRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{_testContext.AanvraagId}/bevestiging");
            s.WithRequestHeader(WellknownHeaderNames.Namens, Vereniging);

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(ExceptionMessages.RelatieaanvraagWerdAlIngetrokken);
        });
    }

    [Fact]
    public async ValueTask Refusing_After_The_Intrekking_Then_400()
    {
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new WeigerRelatieaanvraagRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{_testContext.AanvraagId}/weigering");
            s.WithRequestHeader(WellknownHeaderNames.Namens, Vereniging);

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(ExceptionMessages.RelatieaanvraagWerdAlIngetrokken);
        });
    }

    [Fact]
    public async ValueTask Without_vr_namens_Then_400()
    {
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new TrekRelatieaanvraagInRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{_testContext.AanvraagId}/intrekking");

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain($"{WellknownHeaderNames.Namens} is verplicht.");
        });
    }

    private async Task TrekIn(string aanvraagId, string namens, string expectedMessage) =>
        await _testContext.ApiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post.Json(new TrekRelatieaanvraagInRequest(), JsonStyle.Mvc)
                .ToUrl($"/v1/relatieaanvragen/{aanvraagId}/intrekking");
            s.WithRequestHeader(WellknownHeaderNames.Namens, namens);

            s.StatusCodeShouldBe(HttpStatusCode.BadRequest);
            s.ContentShouldContain(expectedMessage);
        });
}
