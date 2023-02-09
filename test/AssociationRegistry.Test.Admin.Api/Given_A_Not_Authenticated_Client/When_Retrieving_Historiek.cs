namespace AssociationRegistry.Test.Admin.Api.Given_A_Not_Authenticated_Client;

using System.Net;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using FluentAssertions;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class When_Retrieving_Historiek
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_Retrieving_Historiek(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_It_Returns_401_With_Unauthenticated_Client()
    {
        var response = await _fixture.Clients.Unauthenticated.GetHistoriek("VABCDEFG");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Then_It_Returns_403_With_Unauthorized_Client()
    {
        var response = await _fixture.Clients.Unauthorized.GetHistoriek("VABCDEFG");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
