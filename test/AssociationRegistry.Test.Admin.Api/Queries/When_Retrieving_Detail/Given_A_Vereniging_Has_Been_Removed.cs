namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
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
public class Given_A_Vereniging_Has_Been_Removed
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V059_FeitelijkeVerenigingWerdGeregistreerd_AndRemoved _scenario;
    private readonly HttpResponseMessage _response;

    public Given_A_Vereniging_Has_Been_Removed(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V059FeitelijkeVerenigingWerdGeregistreerdAndRemoved;
        _response = _adminApiClient.GetDetail(_scenario.VCode).GetAwaiter().GetResult();
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
        problemDetails.Detail.Should().Be(ValidationMessages.Status404Deleted);
    }
}
