namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
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
public class Given_All_BasisGegevensWerdenGewijzigd : IAsyncLifetime
{
    private readonly AdminApiClient _adminApiClient;
    private readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario _scenario;

    private HttpResponseMessage _response =  null!;

    private string VCode
        => _scenario.VCode;

    private CommandMetadata Metadata
        => _scenario.Metadata;

    private StreamActionResult Result
        => _scenario.Result;

    private ContactInfoLijstWerdGewijzigd ContactInfoLijstWerdGewijzigd
        => _scenario.ContactInfoLijstWerdGewijzigd;

    public Given_All_BasisGegevensWerdenGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario;
        _adminApiClient = fixture.DefaultClient;
    }

    public async Task InitializeAsync()
    {
        _response = await _adminApiClient.GetHistoriek(VCode);
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(VCode, Result.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(VCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(VCode, Result.Sequence + 10))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_all_gebeurtenissen()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""vCode"": ""{VCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""gebeurtenis"": ""Vereniging werd aangemaakt met naam '{_scenario.VerenigingWerdGeregistreerd.Naam}' door {Metadata.Initiator} op datum {Metadata.Tijdstip.ToBelgianDateAndTime()}"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""Naam vereniging werd gewijzigd naar '{_scenario.NaamWerdGewijzigd.Naam}' door {Metadata.Initiator} op datum {Metadata.Tijdstip.ToBelgianDateAndTime()}"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""Korte naam vereniging werd gewijzigd naar '{_scenario.KorteNaamWerdGewijzigd.KorteNaam}' door {Metadata.Initiator} op datum {Metadata.Tijdstip.ToBelgianDateAndTime()}"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""Korte beschrijving vereniging werd gewijzigd naar '{_scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving}' door {Metadata.Initiator} op datum {Metadata.Tijdstip.ToBelgianDateAndTime()}"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""Startdatum vereniging werd gewijzigd naar '{_scenario.StartdatumWerdGewijzigd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}' door {Metadata.Initiator} op datum {Metadata.Tijdstip.ToBelgianDateAndTime()}"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""ContactInfoLijstWerdGewijzigd"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
