﻿namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek;

using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.EventStore;
using AssociationRegistry.Formats;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using Events;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingWerdGestopt
{
    private readonly AdminApiClient _adminApiClient;
    private readonly StreamActionResult _result;
    private readonly HttpResponseMessage _response;
    private readonly CommandMetadata _metadata;
    private readonly V041_FeitelijkeVerenigingWerdGestopt _scenario;

    public Given_VerenigingWerdGestopt(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V041FeitelijkeVerenigingWerdGestopt;
        _adminApiClient = fixture.DefaultClient;

        _metadata = _scenario.Metadata;
        _result = _scenario.Result;
        _response = _adminApiClient.GetHistoriek(_scenario.VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode, _result.Sequence))
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
    public async Task Then_we_get_registratie_gebeurtenissen()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, pattern: "\"datumLaatsteAanpassing\":\".+\"", replacement: "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""@context"": ""{"http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json"}"",
                ""vCode"": ""{_scenario.VCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Feitelijke vereniging werd geregistreerd met naam '{_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam}'."",
                        ""gebeurtenis"":""FeitelijkeVerenigingWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(VerenigingWerdGeregistreerdData.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd))},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }},
{{
      ""beschrijving"": ""De vereniging werd gestopt met einddatum '2023-10-06'."",
      ""gebeurtenis"": ""VerenigingWerdGestopt"",
      ""data"": {{
        ""einddatum"": ""2023-10-06""
      }},
      ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
    }},
    {{
      ""beschrijving"": ""De einddatum van de vereniging werd gewijzigd naar '2023-10-07'."",
      ""gebeurtenis"": ""EinddatumWerdGewijzigd"",
      ""data"": {{
        ""einddatum"": ""2023-10-07""
      }},
      ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
    }},
                    {{
                        ""beschrijving"": ""Feitelijke vereniging werd gemigreerd naar vereniging zonder eigen rechtspersoonlijkheid."",
                        ""gebeurtenis"":""FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid"",
                        ""data"":{JsonConvert.SerializeObject(new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(_scenario.VCode))},
                         ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
