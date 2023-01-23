namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging.When_WijzigBasisGegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.EventStore;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using AutoFixture;
using FluentAssertions;
using Framework;
using VCodes;
using Xunit;

public class With_A_NonMatching_ETag_Fixture : AdminApiFixture
{
    public HttpResponseMessage Response = null!;
    private readonly string _vCode;
    private readonly Fixture _fixture;
    private const string NieuweVerenigingsNaam = "De nieuwe vereniging";
    private StreamActionResult SaveVersionResult { get; set; } = null!;

    public With_A_NonMatching_ETag_Fixture() : base(
        nameof(With_A_NonMatching_ETag_Fixture))
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

    protected override async Task When()
    {
        var jsonBody = $@"{{""naam"":""{NieuweVerenigingsNaam}"", ""Initiator"": ""OVO000001""}}";
        Response = await AdminApiClient.PatchVereniging(_vCode, jsonBody, SaveVersionResult.Version -1);
    }
}

public class With_A_NonMatching_ETag : IClassFixture<With_A_NonMatching_ETag_Fixture>
{
    private readonly With_A_NonMatching_ETag_Fixture _apiFixture;

    public With_A_NonMatching_ETag(With_A_NonMatching_ETag_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_an_preconditionfailed_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
    }

    [Fact]
    public void Then_it_doesnt_return_a_location_header()
    {
        _apiFixture.Response.Headers.Should().NotContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
    }

    [Fact]
    public void Then_it_doesnt_return_a_sequence_header()
    {
        _apiFixture.Response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }

    [Fact]
    public void Then_it_doesnt_save_the_events()
    {
        var savedEvents = _apiFixture.DocumentStore
            .LightweightSession().Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .ToList();

        savedEvents.Should().HaveCount(0);
    }
}
