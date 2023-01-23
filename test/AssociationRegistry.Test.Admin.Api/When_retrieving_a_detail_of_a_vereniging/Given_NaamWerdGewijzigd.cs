namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.EventStore;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using global::AssociationRegistry.Framework;
using VCodes;
using Xunit;

public class When_Detaile_Given_NaamWerdGewijzigd_Fixture : AdminApiFixture
{
    public readonly string VCode;
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    private readonly CommandMetadata _metadata;

    public When_Detaile_Given_NaamWerdGewijzigd_Fixture() : base(nameof(When_Detaile_Given_NaamWerdGewijzigd_Fixture))
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = fixture.Create<VCode>();
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        _metadata = fixture.Create<CommandMetadata>() with {ExpectedVersion = null};
    }

    public StreamActionResult SaveVersionResult { get; private set; } = null!;
    public HttpResponseMessage Response { get; set; } = null!;

    protected override async Task Given()
    {
        await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd,
            _metadata);
        SaveVersionResult = await AddEvent(
            VCode,
            NaamWerdGewijzigd,
            _metadata);
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.GetDetail(VCode);
    }
}

public class Given_NaamWerdGewijzigd : IClassFixture<When_Detaile_Given_NaamWerdGewijzigd_Fixture>
{
    private readonly string _vCode;
    private readonly When_Detaile_Given_NaamWerdGewijzigd_Fixture _werdGewijzigdFixture;
    private readonly AdminApiClient _adminApiClient;

    public Given_NaamWerdGewijzigd(When_Detaile_Given_NaamWerdGewijzigd_Fixture werdGewijzigdFixture)
    {
        _werdGewijzigdFixture = werdGewijzigdFixture;
        _vCode = werdGewijzigdFixture.VCode;
        _adminApiClient = werdGewijzigdFixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _werdGewijzigdFixture.SaveVersionResult.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _werdGewijzigdFixture.SaveVersionResult.Sequence + 1))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var content = await _werdGewijzigdFixture.Response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
        {{
            ""vereniging"": {{
                    ""vCode"": ""{_werdGewijzigdFixture.VCode}"",
                    ""naam"": ""{_werdGewijzigdFixture.NaamWerdGewijzigd.Naam}"",
                    ""korteNaam"": ""{_werdGewijzigdFixture.VerenigingWerdGeregistreerd.KorteNaam}"",
                    ""korteBeschrijving"": ""{_werdGewijzigdFixture.VerenigingWerdGeregistreerd.KorteBeschrijving}"",
                    ""kboNummer"": ""{_werdGewijzigdFixture.VerenigingWerdGeregistreerd.KboNummer}"",
                    ""startdatum"": ""{_werdGewijzigdFixture.VerenigingWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
                    ""status"": ""Actief"",
                    ""contactInfoLijst"": [{string.Join(',', _werdGewijzigdFixture.VerenigingWerdGeregistreerd.ContactInfoLijst!.Select(x => $@"{{
                        ""contactnaam"": ""{x.Contactnaam}"",
                        ""email"": ""{x.Email}"",
                        ""telefoon"": ""{x.Telefoon}"",
                        ""website"": ""{x.Website}"",
                        ""socialMedia"": ""{x.SocialMedia}""
                    }}"))}
                    ],
                    ""locaties"":[{string.Join(',', _werdGewijzigdFixture.VerenigingWerdGeregistreerd.Locaties!.Select(x => $@"{{
                        ""locatietype"": ""{x.Locatietype}"",
                        { (x.Hoofdlocatie ? $"\"hoofdlocatie\": {x.Hoofdlocatie.ToString().ToLower()}," : string.Empty) }
                        ""adres"": ""{x.ToAdresString()}"",
                        ""naam"": ""{x.Naam}"",
                        ""straatnaam"": ""{x.Straatnaam}"",
                        ""huisnummer"": ""{x.Huisnummer}"",
                        ""busnummer"": ""{x.Busnummer}"",
                        ""postcode"": ""{x.Postcode}"",
                        ""gemeente"": ""{x.Gemeente}"",
                        ""land"": ""{x.Land}""
                    }}"))}
                    ]
                }},
                ""metadata"": {{
                    ""datumLaatsteAanpassing"": """"
                }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
