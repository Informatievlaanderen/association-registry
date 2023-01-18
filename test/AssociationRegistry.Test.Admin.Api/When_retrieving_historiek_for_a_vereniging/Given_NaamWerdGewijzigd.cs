namespace AssociationRegistry.Test.Admin.Api.When_retrieving_historiek_for_a_vereniging;

using System.Net;
using System.Text.RegularExpressions;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using Framework;
using VCodes;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_NaamWerdGewijzigd_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    public readonly string VCode;
    private readonly VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd;
    private readonly NaamWerdGewijzigd _naamWerdGewijzigd;
    public CommandMetadata? Metadata;

    public Given_NaamWerdGewijzigd_Fixture() : base(nameof(Given_NaamWerdGewijzigd_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        VCode = _fixture.Create<VCode>();
        _verenigingWerdGeregistreerd = _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        _naamWerdGewijzigd = _fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
    }

    public long Sequence { get; private set; }

    public override async Task InitializeAsync()
    {
        Metadata = _fixture.Create<CommandMetadata>();
        await AddEvent(
            VCode,
            _verenigingWerdGeregistreerd,
            Metadata);
        Sequence = await AddEvent(
            VCode,
            _naamWerdGewijzigd,
            Metadata);
    }
}

public class Given_NaamWerdGewijzigd : IClassFixture<Given_NaamWerdGewijzigd_Fixture>
{
    private readonly string _vCode;
    private readonly Given_NaamWerdGewijzigd_Fixture _fixture;
    private readonly AdminApiClient _adminApiClient;

    public Given_NaamWerdGewijzigd(Given_NaamWerdGewijzigd_Fixture fixture)
    {
        _fixture = fixture;
        _vCode = fixture.VCode;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_vCode, _fixture.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_vCode, _fixture.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _adminApiClient.GetHistoriek(_vCode);

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""vCode"": ""{_fixture.VCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""gebeurtenis"": ""VerenigingWerdGeregistreerd"",
                        ""initiator"":""{_fixture.Metadata.Initiator}"",
                        ""tijdstip"":""{_fixture.Metadata.Tijdstip}""
                    }},
                    {{
                        ""gebeurtenis"": ""NaamWerdGewijzigd"",
                        ""initiator"":""{_fixture.Metadata.Initiator}"",
                        ""tijdstip"":""{_fixture.Metadata.Tijdstip}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
