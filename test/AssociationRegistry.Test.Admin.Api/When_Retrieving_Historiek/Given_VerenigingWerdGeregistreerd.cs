namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using System.Net;
using System.Text.RegularExpressions;
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
public class Given_VerenigingWerdGeregistreerd
{
    private readonly string _vCode;
    private readonly AdminApiClient _adminApiClient;
    private readonly StreamActionResult _result;
    private readonly HttpResponseMessage _response;
    private readonly CommandMetadata _metadata;

    public Given_VerenigingWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _vCode = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VCode;
        _adminApiClient = fixture.DefaultClient;
        _metadata = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.Metadata;
        _result = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.Result;
        _response = fixture.DefaultClient.GetHistoriek(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_vCode, _result.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_vCode, long.MaxValue))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_registratie_gebeurtenissen()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""vCode"": ""{_vCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""gebeurtenis"": ""VerenigingWerdGeregistreerd"",
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
