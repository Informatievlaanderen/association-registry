namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formats;
using Framework;
using templates;
using Vereniging;

using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd
{
    private readonly V021_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd(GivenEventsFixture fixture)
    {
        _scenario = fixture.V021VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async ValueTask Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                                   .WithType(Verenigingstype.Parse(_scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm))
                                   .WithNaam(_scenario.NaamWerdGewijzigdInKbo.Naam)
                                   .WithKorteNaam(_scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam)
                                   .WithLocatie(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Naam,
                                                _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.ToAdresString(),
                                                _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Postcode,
                                                _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Gemeente,
                                                _scenario.VCode,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
