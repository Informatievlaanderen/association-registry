namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Microsoft.Net.Http.Headers;
using System.Net;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens
{
    private readonly AdminApiClient _adminApiClient;
    private readonly HttpResponseMessage _response;
    private V038_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V038VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens;

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
            ""roepnaam"": ""{_scenario.RoepnaamWerdGewijzigd.Roepnaam}"",
            ""korteNaam"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam}"",
            ""korteBeschrijving"": """",
            ""startdatum"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
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
                    ""waarde"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer}""
                }}],
            ""relaties"":[],
            ""bron"": ""{Bron.KBO.Waarde}"",
        }},
        ""metadata"": {{
            ""datumLaatsteAanpassing"": ""{_scenario.Metadata.Tijdstip.ToBelgianDate()}"",
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
