namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using System.Net;
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
        _response = fixture.DefaultClient.GetDetail(VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_404()
        => _response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    [Fact]
    public async Task Then_we_get_a_detail()
    {
        var content = await _response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

        problemDetails.Detail.Should().NotBeEmpty();
        problemDetails.Detail.Should().Be(ValidationMessages.Status404Detail);
    }
}
