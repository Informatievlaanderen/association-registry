namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
using System.Text.RegularExpressions;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using Microsoft.Net.Http.Headers;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly AdminApiClient _adminApiClient;
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly FeitelijkeVerenigingWerdGeregistreerd _feitelijkeVerenigingWerdGeregistreerd;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _vCode = fixture.V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields.VCode;
        _feitelijkeVerenigingWerdGeregistreerd = fixture.V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields.FeitelijkeVerenigingWerdGeregistreerd;
        _adminApiClient = fixture.DefaultClient;
        _response = fixture.DefaultClient.GetDetail(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields.Result.Sequence))
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
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
{{
    ""@context"": ""{"http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json"}"",
    ""vereniging"": {{
            ""vCode"": ""{_feitelijkeVerenigingWerdGeregistreerd.VCode}"",
            ""type"": {{
                ""code"": ""{Verenigingstype.FeitelijkeVereniging.Code}"",
                ""beschrijving"": ""{Verenigingstype.FeitelijkeVereniging.Beschrijving}"",
            }},
            ""naam"": ""{_feitelijkeVerenigingWerdGeregistreerd.Naam}"",
            ""korteNaam"": """",
            ""korteBeschrijving"": """",
            ""startdatum"": null,
            ""einddatum"": null,
            ""doelgroep"" : {{ ""minimumleeftijd"": {_feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd}, ""maximumleeftijd"": {_feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd} }},
            ""status"": ""Actief"",
            ""isUitgeschrevenUitPubliekeDatastroom"": false,
            ""contactgegevens"": [],
            ""locaties"":[],
            ""vertegenwoordigers"":[],
            ""hoofdactiviteitenVerenigingsloket"":[],
            ""sleutels"":[],
                    ""relaties"":[],
            ""bron"": ""{Bron.Initiator.Waarde}"",
        }},
        ""metadata"": {{
            ""datumLaatsteAanpassing"": """"
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
