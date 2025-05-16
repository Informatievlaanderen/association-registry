namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd_And_UitgeschrevenUitPubliekeDatastroom
{
    private readonly PublicApiClient _publicApiClient;
    private readonly string _vCode;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_And_UitgeschrevenUitPubliekeDatastroom(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _vCode = fixture.V010FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario.VCode;
    }

    [Fact]
    public async Task Then_we_get_a_notFound_response()
    {
        var response = await _publicApiClient.GetDetail(_vCode);
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
}
