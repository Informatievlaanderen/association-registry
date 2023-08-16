namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Microsoft.Net.Http.Headers;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_InvalidData
{
    private readonly AdminApiClient _adminApiClient;
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly HttpResponseMessage _response;
    private V030_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_Invalid_Data _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_InvalidData(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _scenario = fixture.V030VerenigingeMetRechtspersoonlijkheidWerdGeregistreerdWithInvalidData;

        _adminApiClient = fixture.DefaultClient;
        _response = fixture.DefaultClient.GetDetail(_scenario.VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, _scenario.Result.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_scenario.VCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, long.MaxValue))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var vCode = _scenario.VCode;
        var expected = $@"
{{
    ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/detail-vereniging-context.json"}"",
    ""vereniging"": {{
            ""vCode"": ""{vCode}"",
            ""type"": {{
                ""code"": ""{Verenigingstype.Parse(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm).Code}"",
                ""beschrijving"": ""{Verenigingstype.Parse(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm).Beschrijving}"",
            }},
            ""naam"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam}"",
            ""korteNaam"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam}"",
            ""korteBeschrijving"": """",
            ""startdatum"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
            ""doelgroep"" : {{ ""minimumleeftijd"": 0, ""maximumleeftijd"": 150 }},
            ""status"": ""Actief"",
            ""isUitgeschrevenUitPubliekeDatastroom"": false,
            ""contactgegevens"": [{{
                        ""contactgegevenId"": 0,
                        ""type"": ""{_scenario.EmailKonNietOvergenomenWordenUitKbo.Type}"",
                        ""waarde"": ""{_scenario.EmailKonNietOvergenomenWordenUitKbo.Waarde}"",
                        ""beschrijving"": null,
                        ""isPrimair"": false,
                    }},
{{
                        ""contactgegevenId"": 0,
                        ""type"": ""{_scenario.WebsiteKonNietOvergenomenWordenUitKbo.Type}"",
                        ""waarde"": ""{_scenario.WebsiteKonNietOvergenomenWordenUitKbo.Waarde}"",
                        ""beschrijving"": null,
                        ""isPrimair"": false,}},
{{

                        ""contactgegevenId"": 0,
                        ""type"": ""{_scenario.TelefoonKonNietOvergenomenWordenUitKbo.Type}"",
                        ""waarde"": ""{_scenario.TelefoonKonNietOvergenomenWordenUitKbo.Waarde}"",
                        ""beschrijving"": null,
                        ""isPrimair"": false,}}],
            ""locaties"":[
                {{
                ""locatieId"": {_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId},
                ""locatietype"": ""Maatschappelijke zetel volgens KBO"",
                ""isPrimair"": false,
                ""naam"": ""{string.Empty}"",
                ""adres"": {{
                    ""straatnaam"": ""Stationsstraat"",
                    ""huisnummer"": ""1"",
                    ""busnummer"": ""B"",
                    ""postcode"": ""1790"",
                    ""gemeente"": ""Affligem"",
                    ""land"": ""België""
                }},
                ""adresvoorstelling"": ""Stationsstraat 1 bus B, 1790 Affligem, België"",
                ""adresId"": null
            }}
            ],
            ""vertegenwoordigers"":[],
            ""hoofdactiviteitenVerenigingsloket"":[],
            ""sleutels"":[
                {{
                    ""bron"": ""KBO"",
                    ""waarde"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer}""
                }}],
            ""relaties"":[],
        }},
        ""metadata"": {{
            ""datumLaatsteAanpassing"": ""{_scenario.Metadata.Tijdstip.ToBelgianDate()}"",
            ""beheerBasisUri"": ""/verenigingen/kbo/{vCode}"",
        }}
}}
";

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public void Then_it_returns_an_etag_header()
    {
        _response.Headers.ETag.Should().NotBeNull();
        var etagValues = _response.Headers.GetValues(HeaderNames.ETag).ToList();
        etagValues.Should().HaveCount(expected: 1);
        var etag = etagValues[index: 0];
        etag.Should().StartWith("W/\"").And.EndWith("\"");
    }
}
