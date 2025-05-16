namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_BasisGegevensWerdenGewijzigd
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V003_BasisgegevensWerdenGewijzigdScenario _scenario;

    public Given_BasisGegevensWerdenGewijzigd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V003BasisgegevensWerdenGewijzigdScenario;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.Should().BeSuccessful();
    }

    [Fact]
    public async ValueTask Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async ValueTask Then_we_get_a_detail_vereniging_response_with_the_new_basisgegevens()
    {
        var responseMessage = await _publicApiClient.GetDetail(_scenario.VCode);

        var content = await responseMessage.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                          .WithNaam(_scenario.NaamWerdGewijzigd.Naam)
                          .WithKorteNaam(_scenario.KorteNaamWerdGewijzigd.KorteNaam)
                          .WithKorteBeschrijving(_scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving)
                          .WithStartdatum(_scenario.StartdatumWerdGewijzigd.Startdatum)
                          .WithDoelgroep(_scenario.VCode, _scenario.DoelgroepWerdGewijzigd.Doelgroep.Minimumleeftijd,
                                         _scenario.DoelgroepWerdGewijzigd.Doelgroep.Maximumleeftijd)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
