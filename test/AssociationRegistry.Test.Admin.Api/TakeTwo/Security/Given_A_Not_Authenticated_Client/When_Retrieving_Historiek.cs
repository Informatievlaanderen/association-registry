namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Security.Given_A_Secured_Api;

using System.Net;
using FluentAssertions;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class When_Retrieving_Historiek
{
    private readonly GivenEventsFixture _fixture;

    public When_Retrieving_Historiek(GivenEventsFixture fixture)
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
