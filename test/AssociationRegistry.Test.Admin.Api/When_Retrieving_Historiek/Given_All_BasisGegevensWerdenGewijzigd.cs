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

    private HttpResponseMessage _response = null!;

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
    public async Task Then_we_get_all_beschrijvingsen()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""vCode"": ""{VCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Vereniging werd geregistreerd met naam '{_scenario.VerenigingWerdGeregistreerd.Naam}'."",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Naam werd gewijzigd naar '{_scenario.NaamWerdGewijzigd.Naam}'."",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Korte naam werd gewijzigd naar '{_scenario.KorteNaamWerdGewijzigd.KorteNaam}'."",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Korte beschrijving werd gewijzigd naar '{_scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving}'."",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Startdatum werd gewijzigd naar '{_scenario.StartdatumWerdGewijzigd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}'."",
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {string.Join(",", ContactInfoLijstWerdGewijzigd.Verwijderingen.Select(v => $@"
                        {{
                            ""beschrijving"": ""Contactinfo met naam '{v.Contactnaam}' werd verwijderd."",
                            ""initiator"":""{Metadata.Initiator}"",
                            ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                        }}
                    "))},
                    {string.Join(",", ContactInfoLijstWerdGewijzigd.Wijzigingen.Select(w => $@"
                        {{
                            ""beschrijving"": ""Contactinfo met naam '{w.Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{w.Email}'."",
                            ""initiator"":""{Metadata.Initiator}"",
                            ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                        }},{{
                            ""beschrijving"": ""Contactinfo met naam '{w.Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{w.Telefoon}'."",
                            ""initiator"":""{Metadata.Initiator}"",
                            ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                        }},{{
                            ""beschrijving"": ""Contactinfo met naam '{w.Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{w.Website}'."",
                            ""initiator"":""{Metadata.Initiator}"",
                            ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                        }},{{
                            ""beschrijving"": ""Contactinfo met naam '{w.Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{w.SocialMedia}'."",
                            ""initiator"":""{Metadata.Initiator}"",
                            ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                        }}
                    "))},{string.Join(",", ContactInfoLijstWerdGewijzigd.Toevoegingen.Select(t => $@"
                        {{
                            ""beschrijving"": ""Contactinfo met naam '{t.Contactnaam}' werd toegevoegd."",
                            ""initiator"":""{Metadata.Initiator}"",
                            ""tijdstip"":""{Metadata.Tijdstip.ToBelgianDateAndTime()}""
                        }}
                    "))}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
