namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging;

using System.Net;
using System.Text.RegularExpressions;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using global::AssociationRegistry.Framework;
using VCodes;
using Xunit;

public class Given_A_Vereniging_With_A_Changed_Naam_Fixture : AdminApiFixture
{
    public readonly string VCode;
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public Given_A_Vereniging_With_A_Changed_Naam_Fixture() : base(nameof(Given_A_Vereniging_With_A_Changed_Naam_Fixture))
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = fixture.Create<VCode>();
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>();
    }

    public long Sequence { get; private set; }

    public override async Task InitializeAsync()
    {
        await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd,
            Metadata);
        Sequence = await AddEvent(
            VCode,
            NaamWerdGewijzigd,
            Metadata);
    }
}

public class Given_A_Vereniging_With_A_Changed_Naam : IClassFixture<Given_A_Vereniging_With_A_Changed_Naam_Fixture>
{
    private readonly string _vCode;
    private readonly Given_A_Vereniging_With_A_Changed_Naam_Fixture _fixture;
    private readonly AdminApiClient _adminApiClient;

    public Given_A_Vereniging_With_A_Changed_Naam(Given_A_Vereniging_With_A_Changed_Naam_Fixture fixture)
    {
        _fixture = fixture;
        _vCode = fixture.VCode;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _adminApiClient.GetDetail(_vCode);

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
    {{
        ""vCode"": ""{_fixture.VCode}"",
        ""gebeurtenissen"": [
            {{
                ""gebeurtenis"": ""VerenigingWerdGeregistreerd"",
                ""initiatior"":""{_fixture.Metadata.Initiator}"",
                ""tijdstip"":""{_fixture.Metadata.Tijdstip}""
            }},
            {{
                ""gebeurtenis"": ""NaamWerdGewijzigd"",
                ""initiatior"":""{_fixture.Metadata.Initiator}"",
                ""tijdstip"":""{_fixture.Metadata.Tijdstip}""
            }}
        ]
    }}
";

        content.Should().BeEquivalentJson(expected);
    }
}
