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

public class Given_KorteNaamWerdGewijzigd_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    public readonly string VCode;
    private readonly VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd;
    private readonly KorteNaamWerdGewijzigd _korteNaamWerdGewijzigd;
    public CommandMetadata? Metadata;

    public Given_KorteNaamWerdGewijzigd_Fixture() : base(nameof(Given_KorteNaamWerdGewijzigd_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        VCode = _fixture.Create<VCode>();
        _verenigingWerdGeregistreerd = _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        _korteNaamWerdGewijzigd = _fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
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
            _korteNaamWerdGewijzigd,
            Metadata);
    }
}

public class Given_KorteNaamWerdGewijzigd : IClassFixture<Given_KorteNaamWerdGewijzigd_Fixture>
{
    private readonly string _vCode;
    private readonly Given_KorteNaamWerdGewijzigd_Fixture _fixture;
    private readonly AdminApiClient _adminApiClient;

    public Given_KorteNaamWerdGewijzigd(Given_KorteNaamWerdGewijzigd_Fixture fixture)
    {
        _fixture = fixture;
        _vCode = fixture.VCode;
        _adminApiClient = fixture.AdminApiClient;
    }

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
                        ""initiator"":""{_fixture.Metadata!.Initiator}"",
                        ""tijdstip"":""{_fixture.Metadata.Tijdstip}""
                    }},
                    {{
                        ""gebeurtenis"": ""KorteNaamWerdGewijzigd"",
                        ""initiator"":""{_fixture.Metadata.Initiator}"",
                        ""tijdstip"":""{_fixture.Metadata.Tijdstip}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
