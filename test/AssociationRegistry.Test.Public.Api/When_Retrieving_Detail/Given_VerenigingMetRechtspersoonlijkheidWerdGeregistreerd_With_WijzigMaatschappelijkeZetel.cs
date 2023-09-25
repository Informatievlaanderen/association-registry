namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Events;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formatters;
using Framework;
using System.Text.RegularExpressions;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel
{
    private readonly HttpResponseMessage _response;
    private readonly V017_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel_Scenario _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel(GivenEventsFixture fixture)
    {
        _scenario = fixture
           .V017VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelScenario;

        _response = fixture.PublicApiClient.GetDetail(_scenario
                                                     .VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_detail_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                          .WithLocatie(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
                                       _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.Naam,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString(),
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Straatnaam,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Huisnummer,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Busnummer,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Gemeente,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Land,
                                       _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.IsPrimair
                           )
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
