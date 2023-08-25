namespace AssociationRegistry.Test.Admin.Api.When_Searching;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_RoepnaamWerdGewijzigd
{
    private readonly V038_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens _scenario;
    private readonly string _goldenMasterWithOneVereniging;
    private readonly AdminApiClient _adminApiClient;

    public Given_RoepnaamWerdGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V038VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens;
        _adminApiClient = fixture.AdminApiClient;

        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"files.{GetType().Name}_{nameof(Then_we_retrieve_one_vereniging_matching_the_vCode_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var expected = $@"
{{
    ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/zoek-verenigingen-context.json"}"",
    ""verenigingen"": [{{
            ""vCode"": ""{_scenario.VCode}"",
            ""type"": {{
                ""code"": ""{Verenigingstype.Parse(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm).Code}"",
                ""beschrijving"": ""{Verenigingstype.Parse(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm).Beschrijving}"",
            }},
            ""naam"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam}"",
            ""roepnaam"": ""{_scenario.RoepnaamWerdGewijzigd.Roepnaam}"",
            ""korteNaam"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam}"",
            ""doelgroep"" : {{ ""minimumleeftijd"": 0, ""maximumleeftijd"": 150 }},
            ""locaties"":[],
            ""hoofdactiviteitenVerenigingsloket"":[],
            ""sleutels"":[
                {{
                    ""bron"": ""KBO"",
                    ""waarde"": ""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer}""
                }}],
            ""links"": {{
                ""detail"": ""http: //127.0.0.1:11004/v1/verenigingen/V9999015""
            }}
}}],
          ""metadata"": {{
            ""pagination"": {{
              ""totalCount"": 1,
              ""offset"": 0,
              ""limit"": 50
            }}
          }}
}}";

        content.Should().BeEquivalentJson(expected);
    }
}
