namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_Vereniging_Does_Not_Exist
{
    private const string VCode = "V9999999";
    private readonly HttpResponseMessage _response;

    public Given_A_Vereniging_Does_Not_Exist(EventsInDbScenariosFixture fixture)
    {
        _response = fixture.DefaultClient.GetHistoriek(VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_404()
        => _response.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
