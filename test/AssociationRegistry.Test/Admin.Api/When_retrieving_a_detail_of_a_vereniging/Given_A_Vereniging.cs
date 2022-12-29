namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging;

using System.Net;
using System.Text.RegularExpressions;
using Fixtures;
using FluentAssertions;
using global::AssociationRegistry.Framework;
using NodaTime.Extensions;
using Vereniging;
using Xunit;

public class Given_A_Vereniging_Fixture : AdminApiFixture
{
    public const string VCode = "v000001";
    private const string Naam = "Feestcommittee Oudenaarde";

    public Given_A_Vereniging_Fixture() : base(nameof(Given_A_Vereniging_Fixture))
    {
    }

    public long Sequence { get; private set; }

    public override async Task InitializeAsync()
    {
        Sequence = await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam: null,
                KorteBeschrijving: null,
                Startdatum: null,
                KboNummer: null,
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                DateOnly.FromDateTime(DateTime.Today)),
            new CommandMetadata(
                "Een initiator",
                new DateTimeOffset(year: 2022, month: 1, day: 1, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant()));
    }
}

public class Given_A_Vereniging : IClassFixture<Given_A_Vereniging_Fixture>
{
    private const string VCode = "v000001";
    private readonly Given_A_Vereniging_Fixture _fixture;
    private readonly AdminApiClient _adminApiClient;

    public Given_A_Vereniging(Given_A_Vereniging_Fixture fixture)
    {
        _fixture = fixture;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(VCode, _fixture.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(VCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(VCode, _fixture.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _adminApiClient.GetDetail(VCode);

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_A_Vereniging)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
