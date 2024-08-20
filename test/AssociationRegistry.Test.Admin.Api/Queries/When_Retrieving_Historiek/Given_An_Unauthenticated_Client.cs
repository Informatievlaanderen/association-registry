<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Given_An_Unauthenticated_Client.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Given_An_Unauthenticated_Client.cs

using FluentAssertions;
using Framework.Fixtures;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Unauthenticated_Client
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_An_Unauthenticated_Client(EventsInDbScenariosFixture fixture)
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
