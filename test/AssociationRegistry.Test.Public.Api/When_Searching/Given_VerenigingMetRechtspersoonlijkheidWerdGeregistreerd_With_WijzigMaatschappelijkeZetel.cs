﻿namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formats;
using Framework;
using templates;
using Vereniging;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel
{
    private readonly V017_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel_Scenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel(GivenEventsFixture fixture)
    {
        _scenario = fixture.V017VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelScenario;
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
                                   .WithLocatie(Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                                                _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.Naam,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString(),
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Gemeente,
                                                _scenario.VCode,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
                                                _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.IsPrimair
                                    )
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
