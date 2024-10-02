namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Unauthenticated_Client
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_An_Unauthenticated_Client(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_It_Returns_401_With_Unauthenticated_Client()
    {
        var response = await _fixture.Clients.Unauthenticated.Search(string.Empty);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Then_It_Returns_403_With_Unauthorized_Client()
    {
        var response = await _fixture.Clients.Unauthorized.Search(string.Empty);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
