namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using Framework;
using FluentAssertions;
using System.Net;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdVerwijderd
{
    private readonly PublicApiClient _publicApiClient;
    private V018_FeitelijkeVerenigingWerdVerwijderdScenario _scenario;

    public Given_FeitelijkeVerenigingWerdVerwijderd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V018_FeitelijkeVerenigingWerdVerwijderdScenario;
    }

    [Fact]
    public async Task Then_we_get_a_notfound_response()
    {
        var response = await _publicApiClient.GetDetail(_scenario.VCode);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
