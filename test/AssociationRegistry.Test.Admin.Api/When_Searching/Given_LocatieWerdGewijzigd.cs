﻿namespace AssociationRegistry.Test.Admin.Api.When_Searching;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Formatters;
using Framework;
using templates;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_LocatieWerdGewijzigd
{
    private readonly V027_LocatieWerdGewijzigd _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_LocatieWerdGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V027LocatieWerdGewijzigd;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_with_the_changed_Locatie()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v =>
                               {
                                   var l2 = _scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[1];
                                   var l3 = _scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[2];

                                   v.WithVCode(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                                    .WithType(Verenigingstype.FeitelijkeVereniging)
                                    .WithNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam)
                                    .WithKorteNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteNaam)
                                    .WithLocatie(_scenario.LocatieWerdGewijzigd.Locatie.Locatietype,
                                                 _scenario.LocatieWerdGewijzigd.Locatie.Naam,
                                                 _scenario.LocatieWerdGewijzigd.Locatie.Adres.ToAdresString(),
                                                 _scenario.LocatieWerdGewijzigd.Locatie.Adres?.Postcode,
                                                 _scenario.LocatieWerdGewijzigd.Locatie.Adres?.Gemeente,
                                                 _scenario.LocatieWerdGewijzigd.Locatie.IsPrimair)
                                    .WithLocatie(l2.Locatietype, l2.Naam, l2.Adres.ToAdresString(), l2.Adres?.Postcode, l2.Adres?.Gemeente,
                                                 l2.IsPrimair)
                                    .WithLocatie(l3.Locatietype, l3.Naam, l3.Adres.ToAdresString(), l3.Adres?.Postcode, l3.Adres?.Gemeente,
                                                 l3.IsPrimair);

                                   return v;
                               });

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
