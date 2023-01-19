namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging.When_WijzigBasisGegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.EventStore;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using AutoFixture;
using FluentAssertions;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using VCodes;
using Xunit;

public class With_A_Matching_ETag_Fixture : AdminApiFixture2
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
        SaveResult = await AddEvent(
            _vCode,
            _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = _vCode },
            _fixture.Create<CommandMetadata>() with { ExpectedVersion = null }
        );
    }

    private SaveChangesResult SaveResult { get; set; } = null!;

    protected override async Task When()
    {
        var jsonBody = $@"{{""naam"":""{NieuweVerenigingsNaam}"", ""Initiator"": ""OVO000001""}}";
        Response = await AdminApiClient.PatchVereniging(_vCode, jsonBody, SaveResult.Version);
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
        var savedEvents = _apiFixture.DocumentStore
            .LightweightSession().Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .ToList();

        savedEvents.Should().HaveCount(1);
        savedEvents[0].Naam.Should().Be(With_A_Matching_ETag_Fixture.NieuweVerenigingsNaam);
    }
}
