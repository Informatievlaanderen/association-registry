namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_All_BasisGegevensWerdenGewijzigd;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using Fixtures;
using Framework;
using TakeTwo;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_Retrieving_Detail
{
    private readonly string _vCode;
    private readonly AdminApiClient _adminApiClient;
    private readonly StreamActionResult _result;
    private readonly HttpResponseMessage _response;
    private readonly CommandMetadata _metadata;
    private readonly VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd;
    private readonly NaamWerdGewijzigd _naamWerdGewijzigd;
    private readonly KorteNaamWerdGewijzigd _korteNaamWerdGewijzigd;
    private readonly KorteBeschrijvingWerdGewijzigd _korteBeschrijvingWerdGewijzigd;

    public When_Retrieving_Detail(GivenEventsFixture fixture)
    {
        _vCode = fixture.AlleBasisGegevensWerdenGewijzigdScenario.VCode;
        _adminApiClient = fixture.AdminApiClient;
        _verenigingWerdGeregistreerd = fixture.AlleBasisGegevensWerdenGewijzigdScenario.VerenigingWerdGeregistreerd;
        _naamWerdGewijzigd = fixture.AlleBasisGegevensWerdenGewijzigdScenario.NaamWerdGewijzigd;
        _korteNaamWerdGewijzigd = fixture.AlleBasisGegevensWerdenGewijzigdScenario.KorteNaamWerdGewijzigd;
        _korteBeschrijvingWerdGewijzigd = fixture.AlleBasisGegevensWerdenGewijzigdScenario.KorteBeschrijvingWerdGewijzigd;
        _metadata = fixture.AlleBasisGegevensWerdenGewijzigdScenario.Metadata;
        _result = fixture.AlleBasisGegevensWerdenGewijzigdScenario.Result;
        _response = fixture.AdminApiClient.GetDetail(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _result.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, long.MaxValue))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected = $@"
        {{
            ""vereniging"": {{
                    ""vCode"": ""{_vCode}"",
                    ""naam"": ""{_naamWerdGewijzigd.Naam}"",
                    ""korteNaam"": ""{_korteNaamWerdGewijzigd.KorteNaam}"",
                    ""korteBeschrijving"": ""{_korteBeschrijvingWerdGewijzigd.KorteBeschrijving}"",
                    ""kboNummer"": ""{_verenigingWerdGeregistreerd.KboNummer}"",
                    ""startdatum"": ""{_verenigingWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
                    ""status"": ""Actief"",
                    ""contactInfoLijst"": [{string.Join(',', _verenigingWerdGeregistreerd.ContactInfoLijst!.Select(x => $@"{{
                        ""contactnaam"": ""{x.Contactnaam}"",
                        ""email"": ""{x.Email}"",
                        ""telefoon"": ""{x.Telefoon}"",
                        ""website"": ""{x.Website}"",
                        ""socialMedia"": ""{x.SocialMedia}""
                    }}"))}
                    ],
                    ""locaties"":[{string.Join(',', _verenigingWerdGeregistreerd.Locaties!.Select(x => $@"{{
                        ""locatietype"": ""{x.Locatietype}"",
                        {(x.Hoofdlocatie ? $"\"hoofdlocatie\": {x.Hoofdlocatie.ToString().ToLower()}," : string.Empty)}
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
                    ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}""
                }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
