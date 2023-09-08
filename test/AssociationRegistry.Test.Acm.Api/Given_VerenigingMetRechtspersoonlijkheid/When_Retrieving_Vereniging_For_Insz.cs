﻿namespace AssociationRegistry.Test.Acm.Api.Given_VerenigingMetRechtspersoonlijkheid;

using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AcmApiCollection))]
[Category("AcmApi")]
[IntegrationTest]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly HttpResponseMessage _response;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_EventsInDbScenario _scenario;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.VerenigingMetRechtspersoonlijkheidWerdGeregistreerdEventsInDbScenario;
        _response = fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_200()
        => _response.StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public async Task Then_we_get_a_response_with_one_vereniging()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected = $@"
        {{
            ""insz"":""{_scenario.Insz}"",
            ""verenigingen"":[
                {{
                    ""vCode"":""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode}"",
                    ""naam"":""{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam}"",
                }}
            ]
        }}";

        content.Should().BeEquivalentJson(expected);
    }
}
