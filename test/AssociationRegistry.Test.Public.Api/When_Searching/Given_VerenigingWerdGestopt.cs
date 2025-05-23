﻿namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_VerenigingWerdGestopt
{
    private readonly V016_VerenigingWerdGestopt _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingWerdGestopt(GivenEventsFixture fixture)
    {
        _scenario = fixture.V016VerenigingWerdGestopt;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async ValueTask Then_we_retrieve_no_vereniging_matching_the_vCode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = new ZoekVerenigingenResponseTemplate();
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
