﻿namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Fixtures;
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
    private readonly V014_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data_Scenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(GivenEventsFixture fixture)
    {
        _scenario = fixture
           .V014VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllDataScenario;

        _publicApiClient = fixture.PublicApiClient;


    }

    [Fact]
    public async Task Then_we_get_a_detail_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                          .WithContactgegeven(_scenario.EmailWerdOvergenomenUitKBO.Contactgegeventype, _scenario.EmailWerdOvergenomenUitKBO.Waarde,
                                              _scenario.EmailWerdGewijzigd.Beschrijving, _scenario.EmailWerdGewijzigd.IsPrimair)
                          .WithContactgegeven(_scenario.WebsiteWerdOvergenomenUitKBO.Contactgegeventype, _scenario.WebsiteWerdOvergenomenUitKBO.Waarde)
                          .WithContactgegeven(_scenario.TelefoonWerdOvergenomenUitKBO.Contactgegeventype, _scenario.TelefoonWerdOvergenomenUitKBO.Waarde)
                          .WithContactgegeven(_scenario.GSMWerdOvergenomenUitKBO.Contactgegeventype, _scenario.GSMWerdOvergenomenUitKBO.Waarde)
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
