namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Fixtures.GivenEvents;
using FluentAssertions;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingWerdGestopt
{
    private readonly HttpResponseMessage _response;

    public Given_VerenigingWerdGestopt(GivenEventsFixture fixture)
    {
        var vCode = fixture.V016VerenigingWerdGestopt.VCode;

        var publicApiClient = fixture.PublicApiClient;
        _response = publicApiClient.GetDetail(vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_not_found_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
