namespace AssociationRegistry.Test.Acm.Api.Given_All_BasisGegevensWerdenGewijzigd;

using System.Net;
using Fixtures;
using Fixtures.Scenarios;
using Framework;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(nameof(AcmApiCollection))]
[Category("AcmApi")]
[IntegrationTest]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario _scenario;
    private readonly AcmApiClient _client;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario;
        _client = fixture.DefaultClient;
    }

    [Fact]
    public void Then_we_get_a_200()
    {
        var response = _client.GetVerenigingenForInsz(_scenario.Inszs.First()).GetAwaiter().GetResult();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Then_we_get_a_response_with_verenigingen()
    {
        foreach (var insz in _scenario.Inszs)
        {
            var response = _client.GetVerenigingenForInsz(insz).GetAwaiter().GetResult();
            var content = await response.Content.ReadAsStringAsync();

            var expected = $@"
            {{
                ""insz"":""{insz}"",
                ""verenigingen"":[
                    {{
                        ""vCode"":""{_scenario.VerenigingWerdGeregistreerd.VCode}"",
                        ""naam"":""{_scenario.NaamWerdOpnieuwGewijzigd.Naam}"",
                    }}
                ]
            }}";

            content.Should().BeEquivalentJson(expected);
        }
    }

    [Fact]
    public async Task Then_we_get_a_response_for_andere_insz_with_verenigingen()
    {
        foreach (var insz in _scenario.InszsAndereVereniging)
        {
            var response = _client.GetVerenigingenForInsz(insz).GetAwaiter().GetResult();
            var content = await response.Content.ReadAsStringAsync();

            var expected = $@"
            {{
                ""insz"":""{insz}"",
                ""verenigingen"":[
                    {{
                        ""vCode"":""{_scenario.AndereVerenigingWerdGeregistreerd.VCode}"",
                        ""naam"":""{_scenario.NaamAndereVerenigingWerdGewijzigd.Naam}"",
                    }}
                ]
            }}";

            content.Should().BeEquivalentJson(expected);
        }
    }
}
