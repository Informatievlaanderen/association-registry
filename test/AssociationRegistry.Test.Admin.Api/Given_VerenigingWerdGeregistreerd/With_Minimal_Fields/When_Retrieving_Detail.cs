namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.With_Minimal_Fields;

using System.Net;
using System.Text.RegularExpressions;
using Events;
using Fixtures;
using Framework;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_Retrieving_Detail
{
    private readonly string _vCode;
    private readonly VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd;
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly AdminApiClient _adminApiClient;
    private readonly HttpResponseMessage _response;

    public When_Retrieving_Detail(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _vCode = fixture.VerenigingWerdGeregistreerdWithMinimalFieldsEventsInDbScenario.VCode;
        _verenigingWerdGeregistreerd = fixture.VerenigingWerdGeregistreerdWithMinimalFieldsEventsInDbScenario.VerenigingWerdGeregistreerd;
        _adminApiClient = fixture.DefaultClient;
        _response = fixture.DefaultClient.GetDetail(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _fixture.VerenigingWerdGeregistreerdWithMinimalFieldsEventsInDbScenario.Result.Sequence))
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
    ""vereniging"": {{
            ""vCode"": ""{_verenigingWerdGeregistreerd.VCode}"",
            ""naam"": ""{_verenigingWerdGeregistreerd.Naam}"",
            ""korteNaam"": null,
            ""korteBeschrijving"": null,
            ""kboNummer"": null,
            ""startdatum"": null,
            ""status"": ""Actief"",
            ""contactInfoLijst"": [],
            ""locaties"":[],
            ""vertegenwoordigers"":[],
            ""hoofdactiviteitenVerenigingsloket"":[]
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
        etagValues.Should().HaveCount(1);
        var etag = etagValues[0];
        etag.Should().StartWith("W/\"").And.EndWith("\"");
    }
}
