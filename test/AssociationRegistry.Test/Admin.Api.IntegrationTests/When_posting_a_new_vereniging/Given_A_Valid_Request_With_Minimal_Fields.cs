namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen;
using AutoFixture;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Microsoft.AspNetCore.Http.Headers;
using Moq;
using Vereniging;
using Xunit;

public class Given_A_Valid_Request_With_Minimal_Fields_Fixture : AdminApiFixture
{
    public Given_A_Valid_Request_With_Minimal_Fields_Fixture() : base(
        nameof(Given_A_Valid_Request_With_Minimal_Fields_Fixture))
    {
        var fixture = new Fixture();
        var request = new RegistreerVerenigingRequest
        {
            Naam = fixture.Create<string>(),
            Initiator = "OVO000001",
        };
        Content = GetJsonBody(request).AsJsonContent();
        Request = request;
    }

    public RegistreerVerenigingRequest Request { get; }

    public StringContent Content { get; }

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_minimal_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.initiator}}", request.Initiator);
}

public class Given_A_Valid_Request_With_Minimal_Fields : IClassFixture<Given_A_Valid_Request_With_Minimal_Fields_Fixture>
{
    private readonly Given_A_Valid_Request_With_Minimal_Fields_Fixture _apiFixture;

    public Given_A_Valid_Request_With_Minimal_Fields(Given_A_Valid_Request_With_Minimal_Fields_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);

        _apiFixture.DocumentStore.LightweightSession().Events.QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(e => e.Naam == _apiFixture.Request.Naam)
            .Should().HaveCount(1);
    }

    [Fact]
    public async Task Then_it_returns_a_location_header()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);
        response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        response.Headers.Location!.OriginalString.Should().StartWith("https://localhost:11003/v1/verenigingen/V");
    }

    [Fact]
    public async Task Then_it_returns_an_etag_header()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);
        response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = response.Headers.GetValues(WellknownHeaderNames.Sequence);
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }
}
