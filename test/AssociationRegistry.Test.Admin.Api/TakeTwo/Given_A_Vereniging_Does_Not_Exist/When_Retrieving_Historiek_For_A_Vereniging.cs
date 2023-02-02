namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist;

using System.Net;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_Retrieving_Historiek_For_A_Vereniging
{
    private const string VCode = "V9999999";
    private readonly HttpResponseMessage _response;

    public When_Retrieving_Historiek_For_A_Vereniging(EventsInDbScenariosFixture fixture)
    {
        _response = fixture.AdminApiClient.GetHistoriek(VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_404()
        => _response.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
