namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Framework;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using Microsoft.Net.Http.Headers;
using System.Net;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly AdminApiClient _adminApiClient;
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly HttpResponseMessage _afdelingResponse;
    private readonly string _vCode;
    private readonly string _moederVCode;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd _moederWerdGeregistreerd;
    private readonly AfdelingWerdGeregistreerd _afdelingWerdGeregistreerd;
    private readonly HttpResponseMessage _moederResponse;
    private readonly CommandMetadata _metadata;

    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _moederWerdGeregistreerd = fixture.V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder.MoederWerdGeregistreerd;
        _afdelingWerdGeregistreerd = fixture.V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder.AfdelingWerdGeregistreerd;

        _vCode = fixture.V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder.VCode;
        _moederVCode = _moederWerdGeregistreerd.VCode;
        _metadata = fixture.V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder.Metadata;

        _adminApiClient = fixture.DefaultClient;
        _afdelingResponse = fixture.DefaultClient.GetDetail(_vCode).GetAwaiter().GetResult();
        _moederResponse = fixture.DefaultClient.GetDetail(_moederVCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder.Result.Sequence))
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
    public async Task Then_we_get_a_detail_afdeling_response()
    {
        var content = await _afdelingResponse.Content.ReadAsStringAsync();

        var expected = $@"
{{
    ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/detail-vereniging-context.json"}"",
    ""vereniging"": {{
            ""vCode"": ""{_afdelingWerdGeregistreerd.VCode}"",
            ""type"": {{
                ""code"": ""{Verenigingstype.Afdeling.Code}"",
                ""beschrijving"": ""{Verenigingstype.Afdeling.Beschrijving}"",
            }},
            ""naam"": ""{_afdelingWerdGeregistreerd.Naam}"",
            ""korteNaam"": """",
            ""korteBeschrijving"": """",
            ""startdatum"": null,
            ""doelgroep"" : {{ ""minimumleeftijd"": {_afdelingWerdGeregistreerd.Doelgroep.Minimumleeftijd}, ""maximumleeftijd"": {_afdelingWerdGeregistreerd.Doelgroep.Maximumleeftijd} }},
            ""status"": ""Actief"",
            ""isUitgeschrevenUitPubliekeDatastroom"": false,
            ""contactgegevens"": [],
            ""locaties"":[],
            ""vertegenwoordigers"":[],
            ""hoofdactiviteitenVerenigingsloket"":[],
            ""sleutels"":[],
            ""relaties"":[
            {{
                ""type"": ""Is afdeling van"",
                ""andereVereniging"": {{
                    ""kboNummer"": ""{_moederWerdGeregistreerd.KboNummer}"",
                    ""vCode"": ""{_moederWerdGeregistreerd.VCode}"",
                    ""naam"": ""{_moederWerdGeregistreerd.Naam}"",
                    ""detail"": ""{"http://127.0.0.1:11004/v1/verenigingen/" + _moederWerdGeregistreerd.VCode}"",
                }},
            }}
            ],
            ""bron"": ""{Bron.Initiator.Waarde}"",
        }},
        ""metadata"": {{
            ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}"",
        }}
        }}
";

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_we_get_a_detail_moeder_response()
    {
        var content = await _moederResponse.Content.ReadAsStringAsync();

        var expected = $@"
{{
    ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/detail-vereniging-context.json"}"",
    ""vereniging"": {{
            ""vCode"": ""{_moederVCode}"",
            ""type"": {{
                ""code"": ""{Verenigingstype.Parse(_moederWerdGeregistreerd.Rechtsvorm).Code}"",
                ""beschrijving"": ""{Verenigingstype.Parse(_moederWerdGeregistreerd.Rechtsvorm).Beschrijving}"",
            }},
            ""naam"": ""{_moederWerdGeregistreerd.Naam}"",
            ""roepnaam"": """",
            ""korteNaam"": ""{_moederWerdGeregistreerd.KorteNaam}"",
            ""korteBeschrijving"": """",
            ""startdatum"": ""{_moederWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
            ""doelgroep"" : {{ ""minimumleeftijd"": 0, ""maximumleeftijd"": 150 }},
            ""status"": ""Actief"",
            ""isUitgeschrevenUitPubliekeDatastroom"": false,
            ""contactgegevens"": [],
            ""locaties"":[],
            ""vertegenwoordigers"":[],
            ""hoofdactiviteitenVerenigingsloket"":[],
            ""sleutels"":[
                {{
                    ""bron"": ""KBO"",
                    ""waarde"": ""{_moederWerdGeregistreerd.KboNummer}""
                }}],
            ""relaties"":[
            {{
                ""type"": ""Heeft als afdeling"",
                ""andereVereniging"": {{
                    ""kboNummer"": """",
                    ""vCode"": ""{_afdelingWerdGeregistreerd.VCode}"",
                    ""naam"": ""{_afdelingWerdGeregistreerd.Naam}"",
                    ""detail"": ""{"http://127.0.0.1:11004/v1/verenigingen/" + _afdelingWerdGeregistreerd.VCode}"",
                }},
            }}
            ],
            ""bron"": ""{Bron.KBO.Waarde}"",
        }},
        ""metadata"": {{
            ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}"",
        }}
        }}
";

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public void Then_it_returns_an_etag_header()
    {
        _afdelingResponse.Headers.ETag.Should().NotBeNull();
        var etagValues = _afdelingResponse.Headers.GetValues(HeaderNames.ETag).ToList();
        etagValues.Should().HaveCount(expected: 1);
        var etag = etagValues[index: 0];
        etag.Should().StartWith("W/\"").And.EndWith("\"");
    }
}
