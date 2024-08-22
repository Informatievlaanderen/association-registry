namespace AssociationRegistry.Test.Acm.Api.Given_All_BasisGegevensWerdenGewijzigd;

using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using System.Net;
using templates;
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

        helper.WriteLine(
            $"Vereniging: {_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode} met naam {_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam}");

        foreach (var vertegenwoordiger in _scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers)
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

        var expected =
            new VerenigingenPerInszResponseTemplate()
               .WithInsz(_scenario.Insz)
               .WithVereniging(
                    _scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
                    _scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Single(s => s.Insz == _scenario.Insz)
                             .VertegenwoordigerId,
                    _scenario.NaamWerdGewijzigd.Naam,
                    _scenario.Verenigingstype
                );

        content.Should().BeEquivalentJson(expected);
    }
}
