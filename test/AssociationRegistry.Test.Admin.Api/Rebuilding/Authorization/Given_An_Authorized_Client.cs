namespace AssociationRegistry.Test.Admin.Api.Rebuilding.Authorization;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures.MinimalApi;
using FluentAssertions;
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
        _client = fixture.AdminApiClients.SuperAdmin;
    }

    [Fact]
    public async Task Then_Statuscode_Is_Ok()
    {
        var response = await _client.RebuildAllAdminProjections(CancellationToken.None);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
