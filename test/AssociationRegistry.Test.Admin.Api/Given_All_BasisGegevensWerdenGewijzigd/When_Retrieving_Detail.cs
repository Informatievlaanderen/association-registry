namespace AssociationRegistry.Test.Admin.Api.Given_All_BasisGegevensWerdenGewijzigd;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using EventStore;
using AssociationRegistry.Framework;
using Fixtures;
using Framework;
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
    private readonly StartDatumWerdGewijzigd _startDatumWerdGewijzigd;

    public When_Retrieving_Detail(EventsInDbScenariosFixture fixture)
    {
        _vCode = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario.VCode;
        _adminApiClient = fixture.DefaultClient;
        _verenigingWerdGeregistreerd = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario.VerenigingWerdGeregistreerd;
        _naamWerdGewijzigd = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario.NaamWerdGewijzigd;
        _korteNaamWerdGewijzigd = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario.KorteNaamWerdGewijzigd;
        _korteBeschrijvingWerdGewijzigd = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario.KorteBeschrijvingWerdGewijzigd;
        _startDatumWerdGewijzigd = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario.StartDatumWerdGewijzigd;
        _metadata = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario.Metadata;
        _result = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario.Result;
        _response = fixture.DefaultClient.GetDetail(_vCode).GetAwaiter().GetResult();
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
                    ""startdatum"": ""{_startDatumWerdGewijzigd.StartDatum!.Value.ToString(WellknownFormats.DateOnly)}"",
                    ""status"": ""Actief"",
                    ""contactInfoLijst"": [{string.Join(',', _verenigingWerdGeregistreerd.ContactInfoLijst.Select(x => $@"{{
                        ""contactnaam"": ""{x.Contactnaam}"",
                        ""email"": ""{x.Email}"",
                        ""telefoon"": ""{x.Telefoon}"",
                        ""website"": ""{x.Website}"",
                        ""socialMedia"": ""{x.SocialMedia}"",
                        ""primairContactInfo"": {(x.PrimairContactInfo ? "true" : "false")},

                    }}"))}
                    ],
                    ""locaties"":[{string.Join(',', _verenigingWerdGeregistreerd.Locaties.Select(x => $@"{{
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
                    ],
                    ""vertegenwoordigers"":[{string.Join(',', _verenigingWerdGeregistreerd.Vertegenwoordigers.Select(x => $@"{{
                            ""insz"": ""{x.Insz}"",
                            ""voornaam"": ""{x.Voornaam}"",
                            ""achternaam"": ""{x.Achternaam}"",
                            ""rol"": ""{x.Rol}"",
                            ""roepnaam"": ""{x.Roepnaam}"",
                            ""primairContactpersoon"": {(x.PrimairContactpersoon ? "true" : "false")},
                    ""contactInfoLijst"": [{string.Join(',', x.ContactInfoLijst.Select(y => $@"{{
                                ""contactnaam"": ""{y.Contactnaam}"",
                                ""email"": ""{y.Email}"",
                                ""telefoon"": ""{y.Telefoon}"",
                                ""website"": ""{y.Website}"",
                                ""socialMedia"": ""{y.SocialMedia}"",
                                ""primairContactInfo"": {(y.PrimairContactInfo ? "true" : "false")},
                            }}"))}
                            ],
                    }}"))}],
                    ""hoofdactiviteitenVerenigingsloket"":[{string.Join(',', _verenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(x => $@"{{
                        ""code"":""{x.Code}"",
                        ""beschrijving"":""{x.Beschrijving}""
                    }}"))}]
                }},
                ""metadata"": {{
                    ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}""
                }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
