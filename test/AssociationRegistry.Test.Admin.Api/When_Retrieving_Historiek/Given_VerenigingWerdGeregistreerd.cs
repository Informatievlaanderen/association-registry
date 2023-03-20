namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using EventStore;
using AssociationRegistry.Framework;
using Events;
using Fixtures;
using Fixtures.Scenarios;
using Framework;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingWerdGeregistreerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly StreamActionResult _result;
    private readonly HttpResponseMessage _response;
    private readonly CommandMetadata _metadata;

    private readonly VerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario _scenario;
    private string VCode
        => _scenario.VCode;

    private CommandMetadata Metadata
        => _scenario.Metadata;
    private VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd
        => _scenario.VerenigingWerdGeregistreerd;


    public Given_VerenigingWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario;
        _adminApiClient = fixture.DefaultClient;
        _metadata = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.Metadata;
        _result = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.Result;
        _response = fixture.DefaultClient.GetHistoriek(VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(VCode, _result.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(VCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(VCode, long.MaxValue))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_registratie_gebeurtenissen()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""vCode"": ""{VCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""gebeurtenis"": ""Vereniging werd aangemaakt met naam '{VerenigingWerdGeregistreerd.Naam}' door {Metadata.Initiator} op datum {Metadata.Tijdstip.ToBelgianDateAndTime()}"",
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
