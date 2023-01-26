namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging_Does_Not_Exist.When_posting_a_new_vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Fixtures;
using Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_Request_With_Minimal_Fields_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture = new();
    public Given_A_Valid_Request_With_Minimal_Fields_Fixture() : base(
        nameof(Given_A_Valid_Request_With_Minimal_Fields_Fixture))
    {
        Request = new RegistreerVerenigingRequest
        {
            Naam = _fixture.Create<string>(),
            Initiator = "OVO000001",
        };
    }

    public RegistreerVerenigingRequest Request { get; set; }
    public HttpResponseMessage Response { get; set; } = null!;

    protected override Task Given()
        => Task.CompletedTask;

    protected override async Task When()
    {
        Response = await AdminApiClient.RegistreerVereniging(GetJsonBody(Request));
    }

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
    public void Then_it_returns_an_accepted_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        _apiFixture.DocumentStore.LightweightSession().Events.QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(e => e.Naam == _apiFixture.Request.Naam)
            .Should().HaveCount(1);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        _apiFixture.Response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        _apiFixture.Response.Headers.Location!.OriginalString.Should().StartWith("http://127.0.0.1:11004/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {

        _apiFixture.Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = _apiFixture.Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Then_we_get_an_etag_header()
    {
        _apiFixture.Response.Headers.ShouldHaveValidEtagHeader();
    }
}
