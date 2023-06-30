namespace AssociationRegistry.Test.Admin.Api.Given_A_CorelationIdMiddleware;

using System.Net;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[UnitTest]
[System.ComponentModel.Category("Middleware")]
[Collection(nameof(AdminApiCollection))]
public class When_No_CorrelationId_Is_Given
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_No_CorrelationId_Is_Given(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_It_Returns_A_400_Response()
    {
        var testClient = _fixture.Clients.GetAuthenticatedHttpClient();

        var response = await testClient.GetAsync("/v1/verenigingen/zoeken");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Be("");
    }
}
