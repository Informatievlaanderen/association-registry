namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
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
[Category("AdminApi")]
[IntegrationTest]
public class Given_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly AdminApiClient _adminApiClient;
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly HttpResponseMessage _afdelingResponse;
    private readonly string _vCode;
    private readonly AfdelingWerdGeregistreerd _afdelingWerdGeregistreerd;
    private readonly CommandMetadata _metadata;

    public Given_AfdelingWerdGeregistreerd_With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _afdelingWerdGeregistreerd = fixture.V019AfdelingWerdGeregistreerdWithMinimalFields.AfdelingWerdGeregistreerd;
        _vCode = fixture.V019AfdelingWerdGeregistreerdWithMinimalFields.VCode;
        _metadata = fixture.V019AfdelingWerdGeregistreerdWithMinimalFields.Metadata;
        _adminApiClient = fixture.DefaultClient;
        _afdelingResponse = fixture.DefaultClient.GetDetail(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.V019AfdelingWerdGeregistreerdWithMinimalFields.Result.Sequence))
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
                    ""kboNummer"": ""{_afdelingWerdGeregistreerd.Moedervereniging.KboNummer}"",
                    ""vCode"": ""{_afdelingWerdGeregistreerd.Moedervereniging.VCode}"",
                    ""naam"": ""{_afdelingWerdGeregistreerd.Moedervereniging.Naam}"",
                    ""detail"": ""{"http://127.0.0.1:11004/v1/verenigingen/" + _afdelingWerdGeregistreerd.Moedervereniging.VCode}"",
                }},
            }}
            ],
        }},
        ""metadata"": {{
            ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}"",
            ""beheerBasisUri"": ""/verenigingen/{_vCode}"",
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
