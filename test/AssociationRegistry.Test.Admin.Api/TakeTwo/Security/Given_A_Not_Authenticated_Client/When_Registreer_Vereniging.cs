namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Security.Given_A_Secured_Api;

using System.Net;
using FluentAssertions;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class When_Registreer_Vereniging
{
    private readonly GivenEventsFixture _fixture;

    public When_Registreer_Vereniging(GivenEventsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_It_Returns_401_With_Unauthenticated_Client()
    {
        var response = await _fixture.Clients.Unauthenticated.RegistreerVereniging(string.Empty);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Then_It_Returns_403_With_Unauthorized_Client()
    {
        var response = await _fixture.Clients.Unauthorized.RegistreerVereniging(string.Empty);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
