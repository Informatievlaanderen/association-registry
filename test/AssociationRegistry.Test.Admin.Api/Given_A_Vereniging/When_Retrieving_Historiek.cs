namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging;

using System.Net;
using AssociationRegistry.EventStore;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using Framework;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

public class When_Retrieving_Historiek_Fixture : AdminApiFixture
{
    public StreamActionResult SaveVersionResult { get; private set; } = null!;
    public const string VCode = "V0001001";

    public When_Retrieving_Historiek_Fixture() : base(nameof(When_Retrieving_Historiek_Fixture))
    {
    }

    public HttpResponseMessage Response { get; set; } = null!;

    protected override async Task Given()
    {
        SaveVersionResult = await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode: VCode,
                Naam: "Feestcommittee Oudenaarde",
                KorteNaam: "FOud",
                KorteBeschrijving: "Het feestcommittee van Oudenaarde",
                Startdatum: DateOnly.FromDateTime(new DateTime(2022, 11, 9)),
                KboNummer: "0123456789",
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>()),
            new CommandMetadata(
                Initiator: "Een initiator",
                Tijdstip: new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero).ToInstant()));
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.GetHistoriek(VCode);
    }
}

public class When_Retrieving_Historiek : IClassFixture<When_Retrieving_Historiek_Fixture>
{
    private readonly When_Retrieving_Historiek_Fixture _fixture;
    private const string VCode = When_Retrieving_Historiek_Fixture.VCode;
    private readonly AdminApiClient _adminApiClient;
    private readonly string _goldenMasterFile;

    public When_Retrieving_Historiek(When_Retrieving_Historiek_Fixture fixture)
    {
        _fixture = fixture;
        _adminApiClient = fixture.AdminApiClient;
        _goldenMasterFile = $"{nameof(When_Retrieving_Historiek)}_{nameof(Then_we_get_a_historiek_response)}";
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(VCode, _fixture.SaveVersionResult.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(VCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(VCode, _fixture.SaveVersionResult.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_historiek_response()
    {
        var content = await _fixture.Response.Content.ReadAsStringAsync();

        var goldenMaster = GetType().GetAssociatedResourceJson(_goldenMasterFile);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
