namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Basisgegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.VCodes;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

//TODO rework into controller test
public class With_A_Matching_ETag_Fixture : AdminApiFixture
{
    public HttpResponseMessage Response = null!;
    private readonly string _vCode;
    private readonly Fixture _fixture;
    public const string NieuweVerenigingsNaam = "De nieuwe vereniging";

    public With_A_Matching_ETag_Fixture() : base(
        nameof(With_A_Matching_ETag_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        _vCode = _fixture.Create<VCode>();
    }

    protected override async Task Given()
    {
        SaveVersionResult = await AddEvent(
            _vCode,
            _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = _vCode },
            _fixture.Create<CommandMetadata>() with { ExpectedVersion = null }
        );
    }

    private StreamActionResult SaveVersionResult { get; set; } = null!;

    protected override async Task When()
    {
        var jsonBody = $@"{{""naam"":""{NieuweVerenigingsNaam}"", ""Initiator"": ""OVO000001""}}";
        Response = await AdminApiClient.PatchVereniging(_vCode, jsonBody, SaveVersionResult.Version);
    }
}

public class With_A_Matching_ETag : IClassFixture<With_A_Matching_ETag_Fixture>
{
    private readonly With_A_Matching_ETag_Fixture _apiFixture;

    public With_A_Matching_ETag(With_A_Matching_ETag_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        _apiFixture.Response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        _apiFixture.Response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_apiFixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
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
    public void Then_it_saves_the_events()
    {
        using var session = _apiFixture.DocumentStore
            .LightweightSession();
        var savedEvents = session.Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .ToList();

        savedEvents.Should().HaveCount(1);
        savedEvents[0].Naam.Should().Be(With_A_Matching_ETag_Fixture.NieuweVerenigingsNaam);
    }
}
