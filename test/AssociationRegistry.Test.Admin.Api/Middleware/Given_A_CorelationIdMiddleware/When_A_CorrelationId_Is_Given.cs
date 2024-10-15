namespace AssociationRegistry.Test.Admin.Api.Middleware.Given_A_CorelationIdMiddleware;

using AssociationRegistry.Admin.Api.Infrastructure;
using FluentAssertions;
using Framework.Fixtures;
using System.Net;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Middleware")]
[Collection(nameof(AdminApiCollection))]
public class When_A_CorrelationId_Is_Given
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_A_CorrelationId_Is_Given(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_It_Does_Not_Returns_A_400_Response()
    {
        var testClient = new AdminApiClient(_fixture.AdminApiClients.GetAuthenticatedHttpClient()).HttpClient;

        var correlationId = Guid.NewGuid().ToString();
        testClient.DefaultRequestHeaders.Remove(WellknownHeaderNames.CorrelationId);
        testClient.DefaultRequestHeaders.Add(WellknownHeaderNames.CorrelationId, correlationId);

        var response = await testClient.GetAsync("/v1/verenigingen/zoeken");

        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_It_Returns_The_CorrelationId()
    {
        var testClient = new AdminApiClient(_fixture.AdminApiClients.GetAuthenticatedHttpClient()).HttpClient;

        var correlationId = Guid.NewGuid().ToString();
        testClient.DefaultRequestHeaders.Remove(WellknownHeaderNames.CorrelationId);
        testClient.DefaultRequestHeaders.Add(WellknownHeaderNames.CorrelationId, correlationId);

        var response = await testClient.GetAsync("/v1/verenigingen/zoeken");

        response.Headers.TryGetValues(WellknownHeaderNames.CorrelationId, out var value);

        value!.SingleOrDefault().Should().BeEquivalentTo(correlationId);
    }
}
