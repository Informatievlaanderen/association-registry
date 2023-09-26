﻿namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Framework;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using templates;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_LocatieWerdVerwijderd
{
    private readonly V012_LocatieWerdVerwijderdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_LocatieWerdVerwijderd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V012LocatieWerdVerwijderdScenario;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vcode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging()
                          .WithVCode(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                          .WithType(Verenigingstype.FeitelijkeVereniging)
                          .WithNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam)
                          .And().Build();

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
