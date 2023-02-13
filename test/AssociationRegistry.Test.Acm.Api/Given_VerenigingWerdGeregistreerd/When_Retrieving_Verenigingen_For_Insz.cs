namespace AssociationRegistry.Test.Acm.Api.Given_VerenigingWerdGeregistreerd;

using System.Net;
using Fixtures;
using Framework;
using Fixtures.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AcmApiCollection))]
[Category("AcmApi")]
[IntegrationTest]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly HttpResponseMessage _response;
    private readonly VerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario _scenario;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario;
        _response = fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_200()
        => _response.StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public async Task Then_we_get_a_response_with_no_verenigingen()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected = $@"
        {{
            ""insz"":""{_scenario.Insz}"",
            ""verenigingen"":[
                {{
                    ""vcode"":""{_scenario.VerenigingWerdGeregistreerd.VCode}"",
                    ""naam"":""{_scenario.VerenigingWerdGeregistreerd.Naam}"",
                }}
            ]
        }}";

        content.Should().BeEquivalentJson(expected);
    }
}
