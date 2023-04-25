﻿namespace AssociationRegistry.Test.Acm.Api.Given_All_BasisGegevensWerdenGewijzigd;

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
    private readonly HttpResponseMessage _response;
    private readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario _scenario;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _scenario = fixture.AlleBasisGegevensWerdenGewijzigdEventsInDbScenario;
        _response = fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz).GetAwaiter().GetResult();
        helper.WriteLine($"INSZ: {_scenario.Insz}");
        helper.WriteLine($"Vereniging: {_scenario.VerenigingWerdGeregistreerd.VCode} met naam {_scenario.VerenigingWerdGeregistreerd.Naam}");
        foreach (var vertegenwoordiger in _scenario.VerenigingWerdGeregistreerd.Vertegenwoordigers)
        {
            helper.WriteLine($"\tVertegenwoordiger: {vertegenwoordiger.Insz}");
        }
        helper.WriteLine($"Vereniging {_scenario.NaamWerdGewijzigd.VCode} gewijzigd naar {_scenario.NaamWerdGewijzigd.Naam}");
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
                    ""vCode"":""{_scenario.VerenigingWerdGeregistreerd.VCode}"",
                    ""naam"":""{_scenario.NaamWerdGewijzigd.Naam}"",
                }}
            ]
        }}";

        content.Should().BeEquivalentJson(expected);
    }
}
