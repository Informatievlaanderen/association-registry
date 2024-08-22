namespace AssociationRegistry.Test.Admin.Api.E2E.When_Rebuilding;

using FluentAssertions;
using Framework.Fixtures;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Unauthorized_Client
{
    private readonly AdminApiClient _client;

    public Given_An_Unauthorized_Client(EventsInDbScenariosFixture fixture)
    {
        _client = fixture.DefaultClient;
    }

    [Fact]
    public async Task Then_Statuscode_Is_Forbidden()
    {
        var response = await _client.RebuildAllAdminProjections(CancellationToken.None);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
