namespace AssociationRegistry.Test.Admin.Api.Given_A_Not_Authenticated_Client;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_Registreer_Vereniging
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_Registreer_Vereniging(EventsInDbScenariosFixture fixture)
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
