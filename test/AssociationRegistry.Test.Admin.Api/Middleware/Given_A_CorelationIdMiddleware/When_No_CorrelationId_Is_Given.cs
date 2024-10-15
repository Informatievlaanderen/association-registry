namespace AssociationRegistry.Test.Admin.Api.Middleware.Given_A_CorelationIdMiddleware;

using AssociationRegistry.Admin.Api.Infrastructure;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Middleware")]
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
        var testClient = new AdminApiClient(_fixture.AdminApiClients.GetAuthenticatedHttpClient()).HttpClient;

        testClient.DefaultRequestHeaders.Remove(WellknownHeaderNames.CorrelationId);

        var response = await testClient.GetAsync("/v1/verenigingen/zoeken");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Be($"{WellknownHeaderNames.CorrelationId} is verplicht.");
    }
}
