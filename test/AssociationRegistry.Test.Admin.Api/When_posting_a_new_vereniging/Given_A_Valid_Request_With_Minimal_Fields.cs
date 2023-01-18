namespace AssociationRegistry.Test.Admin.Api.When_posting_a_new_vereniging;

using System.Net;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using global::AssociationRegistry.Admin.Api.Infrastructure;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Given_A_Valid_Request_With_Minimal_Fields_Fixture : AdminApiFixture
{
    public Given_A_Valid_Request_With_Minimal_Fields_Fixture() : base(
        nameof(Given_A_Valid_Request_With_Minimal_Fields_Fixture))
    {
    }
}

public class Given_A_Valid_Request_With_Minimal_Fields : IClassFixture<Given_A_Valid_Request_With_Minimal_Fields_Fixture>
{
    private readonly Given_A_Valid_Request_With_Minimal_Fields_Fixture _apiFixture;

    public Given_A_Valid_Request_With_Minimal_Fields(Given_A_Valid_Request_With_Minimal_Fields_Fixture apiFixture)
    {
        _apiFixture = apiFixture;

        var fixture = new Fixture();
        Request = new RegistreerVerenigingRequest
        {
            Naam = fixture.Create<string>(),
            Initiator = "OVO000001",
        };
    }

    private RegistreerVerenigingRequest Request { get; }


    [Fact]
    public async Task Then_it_returns_an_accepted_response()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(GetJsonBody(Request));
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await _apiFixture.AdminApiClient.RegistreerVereniging(GetJsonBody(Request));

        _apiFixture.DocumentStore.LightweightSession().Events.QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(e => e.Naam == Request.Naam)
            .Should().HaveCount(1);
    }

    [Fact]
    public async Task Then_it_returns_a_location_header()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(GetJsonBody(Request));
        response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        response.Headers.Location!.OriginalString.Should().StartWith("http://127.0.0.1:11004/v1/verenigingen/V");
    }

    [Fact]
    public async Task Then_it_returns_an_etag_header()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(GetJsonBody(Request));
        response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_minimal_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.initiator}}", request.Initiator);
}
