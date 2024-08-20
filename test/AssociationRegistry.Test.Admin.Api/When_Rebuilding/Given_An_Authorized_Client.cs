namespace AssociationRegistry.Test.Admin.Api.When_Rebuilding;

using FluentAssertions;
using Framework.Fixtures;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Authorized_Client
{
    private readonly AdminApiClient _client;

    public Given_An_Authorized_Client(EventsInDbScenariosFixture fixture)
    {
        _client = fixture.Clients.SuperAdmin;
    }

    [Fact]
    public async Task Then_Statuscode_Is_Ok()
    {
        var response = await _client.RebuildAllAdminProjections(CancellationToken.None);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
