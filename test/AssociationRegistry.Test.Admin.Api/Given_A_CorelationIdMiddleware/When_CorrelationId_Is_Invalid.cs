﻿namespace AssociationRegistry.Test.Admin.Api.Given_A_CorelationIdMiddleware;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Middleware")]
[Collection(nameof(AdminApiCollection))]
public class When_CorrelationId_Is_Invalid
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_CorrelationId_Is_Invalid(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

    }

    [Fact]
    public async Task Then_It_Returns_A_400_Response()
    {
        var testClient = new AdminApiClient(_fixture.Clients.GetAuthenticatedHttpClient()).HttpClient;

        const string correlationId = "not_a_guid";
        testClient.DefaultRequestHeaders.Remove(WellknownHeaderNames.CorrelationId);
        testClient.DefaultRequestHeaders.Add(WellknownHeaderNames.CorrelationId, correlationId);

        var response = await testClient.GetAsync("/v1/verenigingen/zoeken");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Be($"{WellknownHeaderNames.CorrelationId} moet een geldige GUID zijn.");
    }
}
