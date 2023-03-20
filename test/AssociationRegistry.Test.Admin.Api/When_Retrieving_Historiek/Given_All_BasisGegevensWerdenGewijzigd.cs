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
using VCodes;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_All_BasisGegevensWerdenGewijzigd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly StreamActionResult _result;
    private readonly HttpResponseMessage _response;

    private readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario _scenario;
    private string VCode
        => _scenario.VCode;

    private CommandMetadata Metadata
        => _scenario.Metadata;
    private VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd
        => _scenario.VerenigingWerdGeregistreerd;
    private NaamWerdGewijzigd NaamWerdGewijzigd
        => _scenario.NaamWerdGewijzigd;
    private KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd
        => _scenario.KorteNaamWerdGewijzigd;
    private KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd
        => _scenario.KorteBeschrijvingWerdGewijzigd;
    private StartdatumWerdGewijzigd StartdatumWerdGewijzigd
        => _scenario.StartdatumWerdGewijzigd;
    private ContactInfoLijstWerdGewijzigd ContactInfoLijstWerdGewijzigd
        => _scenario.ContactInfoLijstWerdGewijzigd;

    public Given_All_BasisGegevensWerdenGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario;
        _adminApiClient = fixture.DefaultClient;
        _result = _scenario.Result;
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
        => (await _adminApiClient.GetHistoriek(VCode, _result.Sequence + 10))
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
                        ""gebeurtenis"": ""Vereniging werd aangemaakt met naam '{VerenigingWerdGeregistreerd.Naam}' door {Metadata.Initiator} op datum {Metadata.Tijdstip.ToBelgianDateAndTime()}"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""Naam vereniging werd gewijzigd naar '{NaamWerdGewijzigd.Naam}' door {Metadata.Initiator} op datum {Metadata.Tijdstip.ToBelgianDateAndTime()}"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""KorteNaamWerdGewijzigd"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""KorteBeschrijvingWerdGewijzigd"",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""gebeurtenis"": ""StartdatumWerdGewijzigd"",
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
}
