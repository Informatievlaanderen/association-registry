namespace AssociationRegistry.Test.Admin.Api.To_Controller_Tests;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.VCodes;
using AutoFixture;
using FluentAssertions;
using Xunit;

//TODO rework into controller test
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
        using var session = _apiFixture.DocumentStore
            .LightweightSession();
        var savedEvents = session.Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .ToList();

        savedEvents.Should().HaveCount(0);
    }
}
