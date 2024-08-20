<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Given_VerenigingWerdGestopt.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Given_VerenigingWerdGestopt.cs

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Given_VerenigingWerdGestopt.cs
using Common.Scenarios.EventsInDb;
using EventStore;
using FluentAssertions;
using Framework;
========
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Given_VerenigingWerdGestopt.cs
using Framework.Fixtures;
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
                        ""data"":{JsonConvert.SerializeObject(FeitelijkeVerenigingWerdGeregistreerdData.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd))},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToZuluTime()}""
                    }},
{{
      ""beschrijving"": ""De vereniging werd gestopt met einddatum '2023-10-06'."",
      ""gebeurtenis"": ""VerenigingWerdGestopt"",
      ""data"": {{
        ""einddatum"": ""2023-10-06""
      }},
      ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToZuluTime()}""
    }},
    {{
      ""beschrijving"": ""De einddatum van de vereniging werd gewijzigd naar '2023-10-07'."",
      ""gebeurtenis"": ""EinddatumWerdGewijzigd"",
      ""data"": {{
        ""einddatum"": ""2023-10-07""
      }},
      ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToZuluTime()}""
    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
