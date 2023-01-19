namespace AssociationRegistry.Test.Admin.Api.When_retrieving_historiek_for_a_vereniging;

using System.Text.RegularExpressions;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using Framework;
using VCodes;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class When_Historiek_Given_KorteNaamWerdGewijzigd_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    public readonly string VCode;
    private readonly VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd;
    private readonly KorteNaamWerdGewijzigd _korteNaamWerdGewijzigd;
    public CommandMetadata? Metadata;

    public When_Historiek_Given_KorteNaamWerdGewijzigd_Fixture() : base(nameof(When_Historiek_Given_KorteNaamWerdGewijzigd_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        VCode = _fixture.Create<VCode>();
        _verenigingWerdGeregistreerd = _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        _korteNaamWerdGewijzigd = _fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
    }

    public HttpResponseMessage Response { get; set; } = null!;

    protected override async Task Given()
    {
        Metadata = _fixture.Create<CommandMetadata>() with {ExpectedVersion = null};
        await AddEvent(
            VCode,
            _verenigingWerdGeregistreerd,
            Metadata);
        await AddEvent(
            VCode,
            _korteNaamWerdGewijzigd,
            Metadata);
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.GetHistoriek(VCode);
    }
}

public class Given_KorteNaamWerdGewijzigd : IClassFixture<When_Historiek_Given_KorteNaamWerdGewijzigd_Fixture>
{
    private readonly When_Historiek_Given_KorteNaamWerdGewijzigd_Fixture _fixture;

    public Given_KorteNaamWerdGewijzigd(When_Historiek_Given_KorteNaamWerdGewijzigd_Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var content = await _fixture.Response.Content.ReadAsStringAsync();
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
