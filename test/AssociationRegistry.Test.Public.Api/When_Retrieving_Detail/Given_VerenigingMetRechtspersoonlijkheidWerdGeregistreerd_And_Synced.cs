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
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced
{
    private readonly V021_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced(GivenEventsFixture fixture)
    {
        _scenario = fixture
           .V021VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd;

        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_detail_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new DetailVerenigingResponseTemplate()
                          .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                          .WithNaam(_scenario.NaamWerdGewijzigdInKbo.Naam)
                          .WithKorteNaam(_scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam)
                          .WithDatumLaatsteAanpassing(_scenario.GetCommandMetadata().Tijdstip);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
