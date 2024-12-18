namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek;

using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using Common.Extensions;
using FluentAssertions;
using Formats;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly CommandMetadata _metadata;
    private readonly V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data _scenario;

    public Given_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V029VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllData;

        _vCode = _scenario.VCode;
        _metadata = _scenario.Metadata;

        _adminApiClient = fixture.DefaultClient;
        _response = _adminApiClient.GetHistoriek(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode, _scenario.Result.Sequence))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode, long.MaxValue))
          .StatusCode
          .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_registratie_gebeurtenis_for_moeder()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, pattern: "\"datumLaatsteAanpassing\":\".+\"", replacement: "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""@context"": ""{"http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json"}"",
                ""vCode"": ""{_vCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam}'."",
                        ""gebeurtenis"":""VerenigingMetRechtspersoonlijkheidWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }},
{{
                        ""beschrijving"": ""De locatie met type ‘Maatschappelijke zetel volgens KBO' werd overgenomen uit KBO."",
                        ""gebeurtenis"":""MaatschappelijkeZetelWerdOvergenomenUitKbo"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }},
{{
                        ""beschrijving"": ""Contactgegeven ‘{_scenario.EmailWerdOvergenomenUitKBO.TypeVolgensKbo}' werd overgenomen uit KBO."",
                        ""gebeurtenis"":""ContactgegevenWerdOvergenomenUitKBO"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.EmailWerdOvergenomenUitKBO)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }},{{
                        ""beschrijving"": ""Contactgegeven ‘{_scenario.WebsiteWerdOvergenomenUitKBO.TypeVolgensKbo}' werd overgenomen uit KBO."",
                        ""gebeurtenis"":""ContactgegevenWerdOvergenomenUitKBO"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.WebsiteWerdOvergenomenUitKBO)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }},{{
                        ""beschrijving"": ""Contactgegeven ‘{_scenario.TelefoonWerdOvergenomenUitKBO.TypeVolgensKbo}' werd overgenomen uit KBO."",
                        ""gebeurtenis"":""ContactgegevenWerdOvergenomenUitKBO"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.TelefoonWerdOvergenomenUitKBO)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }},{{
                        ""beschrijving"": ""Contactgegeven ‘{_scenario.GSMWerdOvergenomenUitKBO.TypeVolgensKbo}' werd overgenomen uit KBO."",
                        ""gebeurtenis"":""ContactgegevenWerdOvergenomenUitKBO"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.GSMWerdOvergenomenUitKBO)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }},{{
                        ""beschrijving"": ""Vertegenwoordiger 'Jhon Doo' werd overgenomen uit KBO."",
                        ""gebeurtenis"":""VertegenwoordigerWerdOvergenomenUitKBO"",
                        ""data"":{JsonConvert.SerializeObject(VertegenwoordigerData.Create(_scenario.VertegenwoordigerWerdOvergenomenUitKBO))},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }},{{
                        ""beschrijving"": ""{_scenario.EmailWerdOvergenomenUitKBO.TypeVolgensKbo} '{_scenario.EmailWerdOvergenomenUitKBO.Waarde}' werd gewijzigd."",
                        ""gebeurtenis"": ""ContactgegevenUitKBOWerdGewijzigd"",
                        ""data"": {JsonConvert.SerializeObject(_scenario.EmailWerdGewijzigd)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                      }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
