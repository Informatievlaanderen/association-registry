﻿namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formats;
using Framework;
using templates;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel
{
    private readonly V017_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel_Scenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel(GivenEventsFixture fixture)
    {
        _scenario = fixture
           .V017VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelScenario;

        _publicApiClient = fixture.PublicApiClient;

        _publicApiClient.GetDetail(_scenario
                                  .VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async ValueTask Then_we_get_a_detail_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                          .WithLocatie(
                               _scenario.VCode,
                               _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId.ToString(),
                               _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
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
