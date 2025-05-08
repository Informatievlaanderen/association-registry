namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_ContactgegevenWerdToegevoegd
{
    private readonly PublicApiClient _publicApiClient;
    private readonly HttpResponseMessage _response;
    private readonly V005_ContactgegevenWerdToegevoegdScenario _scenario;

    public Given_ContactgegevenWerdToegevoegd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V005ContactgegevenWerdToegevoegdScenario;
        _response = _publicApiClient.GetDetail(_scenario.VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.GetDetail(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async ValueTask Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async ValueTask Then_we_get_a_detail_vereniging_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                          .WithContactgegeven(_scenario.VCode,
                                              _scenario.ContactgegevenWerdToegevoegd.ContactgegevenId.ToString(),
                                              _scenario.ContactgegevenWerdToegevoegd.Contactgegeventype,
                                              _scenario.ContactgegevenWerdToegevoegd.Waarde,
                                              _scenario.ContactgegevenWerdToegevoegd.Beschrijving,
                                              _scenario.ContactgegevenWerdToegevoegd.IsPrimair)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
