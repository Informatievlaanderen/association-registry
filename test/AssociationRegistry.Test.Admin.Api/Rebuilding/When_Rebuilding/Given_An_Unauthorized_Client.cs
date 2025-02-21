namespace AssociationRegistry.Test.Admin.Api.Rebuilding.When_Rebuilding;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
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
