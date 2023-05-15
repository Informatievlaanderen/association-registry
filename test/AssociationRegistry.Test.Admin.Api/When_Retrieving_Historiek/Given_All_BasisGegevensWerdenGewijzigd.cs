namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using EventStore;
using AssociationRegistry.Framework;
using Fixtures;
using Fixtures.Scenarios;
using Framework;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_All_BasisGegevensWerdenGewijzigd : IAsyncLifetime
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V004_AlleBasisGegevensWerdenGewijzigd _scenario;

    private HttpResponseMessage _response = null!;

    private string VCode
        => _scenario.VCode;

    private CommandMetadata Metadata
        => _scenario.Metadata;

    private StreamActionResult Result
        => _scenario.Result;

    public Given_All_BasisGegevensWerdenGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V004AlleBasisGegevensWerdenGewijzigd;
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
        => (await _adminApiClient.GetHistoriek(VCode, Result.Sequence + 1000))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_all_beschrijvingsen()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""vCode"": ""{VCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Feitelijke vereniging werd geregistreerd met naam '{_scenario.VerenigingWerdGeregistreerd.Naam}'."",
                        ""gebeurtenis"":""VerenigingWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.VerenigingWerdGeregistreerd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Naam werd gewijzigd naar '{_scenario.NaamWerdGewijzigd.Naam}'."",
                        ""gebeurtenis"":""NaamWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.NaamWerdGewijzigd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Korte naam werd gewijzigd naar '{_scenario.KorteNaamWerdGewijzigd.KorteNaam}'."",
                        ""gebeurtenis"":""KorteNaamWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.KorteNaamWerdGewijzigd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Korte beschrijving werd gewijzigd naar '{_scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving}'."",
                        ""gebeurtenis"":""KorteBeschrijvingWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.KorteBeschrijvingWerdGewijzigd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Startdatum werd gewijzigd naar '{_scenario.StartdatumWerdGewijzigd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}'."",
                        ""gebeurtenis"":""StartdatumWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.StartdatumWerdGewijzigd)},
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
