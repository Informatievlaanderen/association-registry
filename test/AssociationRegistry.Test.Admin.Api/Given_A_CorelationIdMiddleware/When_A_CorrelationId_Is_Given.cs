namespace AssociationRegistry.Test.Admin.Api.Given_A_CorelationIdMiddleware;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using Fixtures;
using FluentAssertions;
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
        var testClient = _fixture.Clients.GetAuthenticatedHttpClient();

        var correlationId = Guid.NewGuid().ToString();
        testClient.DefaultRequestHeaders.Add(CorrelationIdMiddleware.CorrelationIdHeader, correlationId);

        var response = await testClient.GetAsync("/v1/verenigingen/zoeken");

        response.StatusCode.Should().NotBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_It_Returns_The_CorrelationId()
    {
        var testClient = _fixture.Clients.GetAuthenticatedHttpClient();

        var correlationId = Guid.NewGuid().ToString();
        testClient.DefaultRequestHeaders.Add(CorrelationIdMiddleware.CorrelationIdHeader, correlationId);

        var response = await testClient.GetAsync("/v1/verenigingen/zoeken");

        response.Headers.TryGetValues(CorrelationIdMiddleware.CorrelationIdHeader, out var value);

        value!.SingleOrDefault().Should().BeEquivalentTo(correlationId);
    }
}
