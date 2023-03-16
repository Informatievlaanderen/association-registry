namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using System.Net;
using AssociationRegistry.Test.Admin.Api.Fixtures;
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
        _response = fixture.DefaultClient.GetHistoriek(VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_404()
        => _response.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
