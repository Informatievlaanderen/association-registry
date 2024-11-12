namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Formats;
using System.Net;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingWerdVerwijderd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly StreamActionResult _result;
    private readonly HttpResponseMessage _response;
    private readonly CommandMetadata _metadata;
    private readonly V059_FeitelijkeVerenigingWerdGeregistreerd_AndRemoved _scenario;

    public Given_VerenigingWerdVerwijderd(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V059FeitelijkeVerenigingWerdGeregistreerdAndRemoved;
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
          ""beschrijving"": ""Deze vereniging werd verwijderd."",
          ""gebeurtenis"": ""VerenigingWerdVerwijderd"",
          ""data"": {{
            ""reden"": ""{_scenario.VerenigingWerdVerwijderd.Reden}""
          }},
          ""initiator"":""{_metadata.Initiator}"",
                            ""tijdstip"":""{_metadata.Tijdstip.FormatAsZuluTime()}""
        }}
                    ]
                }}
            ";

        content.Should().BeEquivalentJson(expected);
    }
}
