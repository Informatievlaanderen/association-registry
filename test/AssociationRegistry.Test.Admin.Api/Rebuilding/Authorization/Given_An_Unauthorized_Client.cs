namespace AssociationRegistry.Test.Admin.Api.Rebuilding.Authorization;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures.MinimalApi;
using FluentAssertions;
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class Given_An_Unauthorized_Client
{
    private readonly AdminApiClient _client;

    public Given_An_Unauthorized_Client(EventsInDbScenariosFixture fixture)
    {
        _client = fixture.DefaultClient;
    }

    [Fact]
    public async ValueTask Then_Statuscode_Is_Forbidden()
    {
        var response = await _client.RebuildAllAdminProjections(CancellationToken.None);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
