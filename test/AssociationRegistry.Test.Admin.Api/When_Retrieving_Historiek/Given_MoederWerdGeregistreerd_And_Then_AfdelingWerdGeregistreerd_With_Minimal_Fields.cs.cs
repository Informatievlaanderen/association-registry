namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Framework;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using Framework;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly AdminApiClient _adminApiClient;
    private readonly HttpResponseMessage _afdelingResponse;
    private readonly string _vCode;
    private readonly string _moederVCode;
    private readonly HttpResponseMessage _moederResponse;
    private readonly CommandMetadata _metadata;

    private readonly V017_AfdelingWerdGeregistreerd_WithMinimalFields_And_Registered_Moeder _scenario;


    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder;
        var moederWerdGeregistreerd = _scenario.MoederWerdGeregistreerd;

        _vCode = _scenario.VCode;
        _moederVCode = moederWerdGeregistreerd.VCode;
        _metadata = _scenario.Metadata;

        _adminApiClient = fixture.DefaultClient;
        _afdelingResponse = _adminApiClient.GetHistoriek(_vCode).GetAwaiter().GetResult();
        _moederResponse = _adminApiClient.GetHistoriek(_moederVCode).GetAwaiter().GetResult();
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
    public async Task Then_we_get_registratie_gebeurtenis()
    {
        var content = await _afdelingResponse.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/historiek-vereniging-context.json"}"",
                ""vCode"": ""{_vCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Afdeling werd geregistreerd met naam '{_scenario.AfdelingWerdGeregistreerd.Naam}'."",
                        ""gebeurtenis"":""AfdelingWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(AfdelingWerdGeregistreerdData.Create(_scenario.AfdelingWerdGeregistreerd))},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_we_get_registratie_gebeurtenis_for_moeder()
    {
        var content = await _moederResponse.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/historiek-vereniging-context.json"}"",
                ""vCode"": ""{_moederVCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{_scenario.MoederWerdGeregistreerd.Naam}'."",
                        ""gebeurtenis"":""VerenigingMetRechtspersoonlijkheidWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.MoederWerdGeregistreerd)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }},
                    {{
                        ""beschrijving"": ""'{_scenario.AfdelingWerdGeregistreerd.Naam}' werd geregistreerd als afdeling."",
                        ""gebeurtenis"":""AfdelingWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(AfdelingWerdGeregistreerdData.Create(_scenario.AfdelingWerdGeregistreerd))},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
