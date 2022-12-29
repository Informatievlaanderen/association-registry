namespace AssociationRegistry.Test.Admin.Api.When_retrieving_historiek_for_a_vereniging;

using System.Net;
using Fixtures;
using FluentAssertions;
using global::AssociationRegistry.Framework;
using NodaTime.Extensions;
using Vereniging;
using Xunit;

public class Given_A_Vereniging_With_Historiek_Fixture : AdminApiFixture
{
    public long Sequence { get; private set; }
    public const string VCode = "v0001001";

    public Given_A_Vereniging_With_Historiek_Fixture() : base(nameof(Given_A_Vereniging_With_Historiek_Fixture))
    {
    }

    public override async Task InitializeAsync()
    {
        Sequence = await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode: VCode,
                Naam: "Feestcommittee Oudenaarde",
                KorteNaam: "FOud",
                KorteBeschrijving: "Het feestcommittee van Oudenaarde",
                Startdatum: DateOnly.FromDateTime(new DateTime(2022, 11, 9)),
                KboNummer: "0123456789",
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                DatumLaatsteAanpassing: DateOnly.FromDateTime(DateTime.Today)),
            new CommandMetadata(
                Initiator: "Een initiator",
                Tijdstip: new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero).ToInstant()));
    }
}

public class Given_A_Vereniging_With_Historiek : IClassFixture<Given_A_Vereniging_With_Historiek_Fixture>
{
    private readonly Given_A_Vereniging_With_Historiek_Fixture _fixture;
    private const string VCode = Given_A_Vereniging_With_Historiek_Fixture.VCode;
    private readonly AdminApiClient _adminApiClient;
    private readonly string _goldenMasterFile;

    public Given_A_Vereniging_With_Historiek(Given_A_Vereniging_With_Historiek_Fixture fixture)
    {
        _fixture = fixture;
        _adminApiClient = fixture.AdminApiClient;
        _goldenMasterFile = $"{nameof(Given_A_Vereniging_With_Historiek)}_{nameof(Then_we_get_a_historiek_response)}";
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(VCode, _fixture.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(VCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(VCode, _fixture.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_historiek_response()
    {
        var responseMessage = await _adminApiClient.GetHistoriek(VCode);

        var content = await responseMessage.Content.ReadAsStringAsync();

        var goldenMaster = GetType().GetAssociatedResourceJson(_goldenMasterFile);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
