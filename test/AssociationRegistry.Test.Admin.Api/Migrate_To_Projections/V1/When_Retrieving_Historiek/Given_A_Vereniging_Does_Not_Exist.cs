namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
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

    [Fact]
    public async ValueTask Then_we_get_a_detail()
    {
        var content = await _response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

        problemDetails.Detail.Should().NotBeEmpty();
        problemDetails.Detail.Should().Be(ValidationMessages.Status404NotFound);
    }
}
