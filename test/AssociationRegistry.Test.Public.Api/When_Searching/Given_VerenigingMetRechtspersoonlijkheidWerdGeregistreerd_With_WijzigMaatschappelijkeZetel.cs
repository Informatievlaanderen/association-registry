﻿namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formatters;
using Framework;
using templates;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
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
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging()
                          .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                          .WithLocatie(Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                                       _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.Naam,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString(),
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode,
                                       _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Gemeente,
                                       _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.IsPrimair
                           )
                          .And()
                          .Build();

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
