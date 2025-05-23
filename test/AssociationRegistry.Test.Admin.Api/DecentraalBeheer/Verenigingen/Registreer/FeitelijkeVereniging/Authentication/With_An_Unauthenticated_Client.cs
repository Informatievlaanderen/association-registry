namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.Authentication;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class With_An_Unauthenticated_Client
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_An_Unauthenticated_Client(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async ValueTask Then_It_Returns_401_With_Unauthenticated_Client()
    {
        var response = await _fixture.AdminApiClients.Unauthenticated.RegistreerFeitelijkeVereniging(string.Empty);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async ValueTask Then_It_Returns_403_With_Unauthorized_Client()
    {
        var response = await _fixture.AdminApiClients.Unauthorized.RegistreerFeitelijkeVereniging(string.Empty);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
