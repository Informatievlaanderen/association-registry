namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.VCodes;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit;

public class When_WijzigBasisGegevens_Fixture : AdminApiFixture
{
    public HttpResponseMessage Response = null!;
    private readonly string _vCode;
    private readonly Fixture _fixture;
    public const string NieuweVerenigingsNaam = "De nieuwe vereniging";
    public const string NieuweKorteNaam = "De nieuwe korte naam";
    public const string NieuweKorteBeschrijving = "De nieuwe korte beschrijving";

    public When_WijzigBasisGegevens_Fixture() : base(
        nameof(When_WijzigBasisGegevens_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        _vCode = _fixture.Create<VCode>();
    }

    protected override async Task Given()
    {
        await AddEvent(
            _vCode,
            _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = _vCode },
            _fixture.Create<CommandMetadata>()
        );
    }

    protected override async Task When()
    {
        var jsonBody = $@"{{
            ""naam"":""{NieuweVerenigingsNaam}"",
            ""korteNaam"":""{NieuweKorteNaam}"",
            ""korteBeschrijving"":""{NieuweKorteBeschrijving}"",
             ""Initiator"": ""OVO000001""}}";
        Response = await AdminApiClient.PatchVereniging(_vCode, jsonBody);
    }
}

public class When_WijzigBasisGegevens : IClassFixture<When_WijzigBasisGegevens_Fixture>
{
    private readonly When_WijzigBasisGegevens_Fixture _apiFixture;

    public When_WijzigBasisGegevens(When_WijzigBasisGegevens_Fixture apiFixture)
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
        _apiFixture.Response.Headers.Should().ContainKey(HeaderNames.Location);
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
        var naamWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .Single();
        var korteNaamWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<KorteNaamWerdGewijzigd>()
            .Single();
        var korteBeschrijvingWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<KorteBeschrijvingWerdGewijzigd>()
            .Single();


        naamWerdGewijzigd.Naam.Should().Be(When_WijzigBasisGegevens_Fixture.NieuweVerenigingsNaam);
        korteNaamWerdGewijzigd.KorteNaam.Should().Be(When_WijzigBasisGegevens_Fixture.NieuweKorteNaam);
        korteBeschrijvingWerdGewijzigd.KorteBeschrijving.Should().Be(When_WijzigBasisGegevens_Fixture.NieuweKorteBeschrijving);
    }

    [Fact]
    public void Then_we_get_an_etag_header()
    {
        _apiFixture.Response.Headers.ShouldHaveValidEtagHeader();
    }
}
