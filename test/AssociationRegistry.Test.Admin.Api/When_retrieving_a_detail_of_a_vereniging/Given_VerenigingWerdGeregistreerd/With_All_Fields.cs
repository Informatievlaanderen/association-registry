namespace AssociationRegistry.Test.Admin.Api.When_retrieving_a_detail_of_a_vereniging.Given_VerenigingWerdGeregistreerd;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using Framework;
using VCodes;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class With_All_Fields_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    public readonly string VCode;
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;

    public With_All_Fields_Fixture() : base(nameof(With_All_Fields_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        VCode = _fixture.Create<VCode>();
        VerenigingWerdGeregistreerd = _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
    }

    public long Sequence { get; private set; }

    public override async Task InitializeAsync()
    {
        var metadata = _fixture.Create<CommandMetadata>();
        Sequence = await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd,
            metadata);
    }
}

public class With_All_Fields : IClassFixture<With_All_Fields_Fixture>
{
    private readonly string _vCode;
    private readonly With_All_Fields_Fixture _fixture;
    private readonly AdminApiClient _adminApiClient;

    public With_All_Fields(With_All_Fields_Fixture fixture)
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
    ""vereniging"": {{
            ""vCode"": ""{_fixture.VCode}"",
            ""naam"": ""{_fixture.VerenigingWerdGeregistreerd.Naam}"",
            ""korteNaam"": ""{_fixture.VerenigingWerdGeregistreerd.KorteNaam}"",
            ""korteBeschrijving"": ""{_fixture.VerenigingWerdGeregistreerd.KorteBeschrijving}"",
            ""kboNummer"": ""{_fixture.VerenigingWerdGeregistreerd.KboNummer}"",
            ""startdatum"": ""{_fixture.VerenigingWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
            ""status"": ""Actief"",
            ""contactInfoLijst"": [{string.Join(',', _fixture.VerenigingWerdGeregistreerd.ContactInfoLijst!.Select(x => $@"{{
                ""contactnaam"": ""{x.Contactnaam}"",
                ""email"": ""{x.Email}"",
                ""telefoon"": ""{x.Telefoon}"",
                ""website"": ""{x.Website}"",
                ""socialMedia"": ""{x.SocialMedia}""
            }}"))}
            ],
            ""locaties"":[{string.Join(',', _fixture.VerenigingWerdGeregistreerd.Locaties!.Select(x => $@"{{
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
