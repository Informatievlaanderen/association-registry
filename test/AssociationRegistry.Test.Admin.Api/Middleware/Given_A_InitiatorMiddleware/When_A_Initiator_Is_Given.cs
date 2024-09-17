namespace AssociationRegistry.Test.Admin.Api.Middleware.Given_A_InitiatorMiddleware;

using AssociationRegistry.Admin.Api.Infrastructure;
using FluentAssertions;
using Framework.Categories;
using Framework.Fixtures;
using System.Net;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Middleware")]
[IntegrationTestToRefactor]
[Collection(nameof(AdminApiCollection))]
public class When_A_Initiator_Is_Given
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_A_Initiator_Is_Given(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_It_Does_Not_Returns_A_400_Response()
    {
        var testClient = new AdminApiClient(_fixture.Clients.GetAuthenticatedHttpClient()).HttpClient;

        const string initiator = "OVO000001";
        testClient.DefaultRequestHeaders.Remove(WellknownHeaderNames.Initiator);
        testClient.DefaultRequestHeaders.Add(WellknownHeaderNames.Initiator, initiator);

        var response = await testClient.GetAsync("/v1/verenigingen/zoeken");

        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
    }
}
