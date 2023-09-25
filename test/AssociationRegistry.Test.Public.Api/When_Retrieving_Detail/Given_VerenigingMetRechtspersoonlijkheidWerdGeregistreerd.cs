namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Events;
using Fixtures;
using System.Text.RegularExpressions;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formatters;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly HttpResponseMessage _response;
    private readonly V014_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data_Scenario _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(GivenEventsFixture fixture)
    {
        _scenario = fixture
           .V014VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllDataScenario;

        _response = fixture.PublicApiClient
                           .GetDetail(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode)
                           .GetAwaiter()
                           .GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_detail_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                          .WithContactgegeven(_scenario.EmailWerdOvergenomenUitKBO.Type, _scenario.EmailWerdOvergenomenUitKBO.Waarde,
                                              _scenario.EmailWerdGewijzigd.Beschrijving, _scenario.EmailWerdGewijzigd.IsPrimair)
                          .WithContactgegeven(_scenario.WebsiteWerdOvergenomenUitKBO.Type, _scenario.WebsiteWerdOvergenomenUitKBO.Waarde)
                          .WithContactgegeven(_scenario.TelefoonWerdOvergenomenUitKBO.Type, _scenario.TelefoonWerdOvergenomenUitKBO.Waarde)
                          .WithContactgegeven(_scenario.GSMWerdOvergenomenUitKBO.Type, _scenario.GSMWerdOvergenomenUitKBO.Waarde)
                          .WithLocatie(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Naam,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString(),
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Straatnaam,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Huisnummer,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Busnummer,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Gemeente,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Land
                           )
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
