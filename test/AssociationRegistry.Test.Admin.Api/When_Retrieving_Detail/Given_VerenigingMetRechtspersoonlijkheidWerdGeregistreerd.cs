namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Framework;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using Microsoft.Net.Http.Headers;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd _verenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    private readonly CommandMetadata _metadata;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.V028VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

        _vCode = fixture.V028VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd.VCode;
        _metadata = fixture.V028VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd.Metadata;

        _adminApiClient = fixture.DefaultClient;
        _response = fixture.DefaultClient.GetDetail(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.V028VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd.Result.Sequence))
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
    public async Task Then_we_get_a_detail_moeder_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected = $@"
{{
    ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/detail-vereniging-context.json"}"",
    ""vereniging"": {{
            ""vCode"": ""{_vCode}"",
            ""type"": {{
                ""code"": ""{Verenigingstype.VZW.Code}"",
                ""beschrijving"": ""{Verenigingstype.VZW.Beschrijving}"",
            }},
            ""naam"": ""{_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam}"",
            ""korteNaam"": ""{_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam}"",
            ""korteBeschrijving"": """",
            ""startdatum"": ""{_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
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
                    ""waarde"": ""{_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer}""
                }}],
            ""relaties"":[            ],
        }},
        ""metadata"": {{
            ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}"",
            ""beheerBasisUri"": ""/verenigingen/kbo/{_vCode}"",
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
