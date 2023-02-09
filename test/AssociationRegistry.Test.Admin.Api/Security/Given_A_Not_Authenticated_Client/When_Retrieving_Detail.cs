namespace AssociationRegistry.Test.Admin.Api.Security.Given_A_Not_Authenticated_Client;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class When_Retrieving_Detail
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_Retrieving_Detail(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_It_Returns_401_With_Unauthenticated_Client()
    {
        var response = await _fixture.Clients.Unauthenticated.GetDetail("VABCDEFG");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Then_It_Returns_403_With_Unauthorized_Client()
    {
        var response = await _fixture.Clients.Unauthorized.GetDetail("VABCDEFG");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
