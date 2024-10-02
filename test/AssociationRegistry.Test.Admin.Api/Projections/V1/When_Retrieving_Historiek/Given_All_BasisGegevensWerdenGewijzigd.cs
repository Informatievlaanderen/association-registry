namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.EventStore;
using AssociationRegistry.Formats;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
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
        content = Regex.Replace(content, pattern: "\"datumLaatsteAanpassing\":\".+\"", replacement: "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""@context"": ""{"http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json"}"",
                ""vCode"": ""{VCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Feitelijke vereniging werd geregistreerd met naam '{_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam}'."",
                        ""gebeurtenis"":""FeitelijkeVerenigingWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(FeitelijkeVerenigingWerdGeregistreerdData.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd))},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToZuluTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Naam werd gewijzigd naar '{_scenario.NaamWerdGewijzigd.Naam}'."",
                        ""gebeurtenis"":""NaamWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.NaamWerdGewijzigd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToZuluTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Korte naam werd gewijzigd naar '{_scenario.KorteNaamWerdGewijzigd.KorteNaam}'."",
                        ""gebeurtenis"":""KorteNaamWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.KorteNaamWerdGewijzigd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToZuluTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Korte beschrijving werd gewijzigd naar '{_scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving}'."",
                        ""gebeurtenis"":""KorteBeschrijvingWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.KorteBeschrijvingWerdGewijzigd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToZuluTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Startdatum werd gewijzigd naar '{_scenario.StartdatumWerdGewijzigd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}'."",
                        ""gebeurtenis"":""StartdatumWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.StartdatumWerdGewijzigd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToZuluTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Vereniging werd uitgeschreven uit de publieke datastroom."",
                        ""gebeurtenis"":""VerenigingWerdUitgeschrevenUitPubliekeDatastroom"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.VerenigingWerdUitgeschrevenUitPubliekeDatastroom)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToZuluTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Vereniging werd ingeschreven in de publieke datastroom."",
                        ""gebeurtenis"":""VerenigingWerdIngeschrevenInPubliekeDatastroom"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.VerenigingWerdIngeschrevenInPubliekeDatastroom)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToZuluTime()}""
                    }},
                    {{
                        ""beschrijving"": ""Doelgroep werd gewijzigd naar '{_scenario.DoelgroepWerdGewijzigd.Doelgroep.Minimumleeftijd} - {_scenario.DoelgroepWerdGewijzigd.Doelgroep.Maximumleeftijd}'."",
                        ""gebeurtenis"":""DoelgroepWerdGewijzigd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.DoelgroepWerdGewijzigd)},
                        ""initiator"":""{Metadata.Initiator}"",
                        ""tijdstip"":""{Metadata.Tijdstip.ToZuluTime()}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
