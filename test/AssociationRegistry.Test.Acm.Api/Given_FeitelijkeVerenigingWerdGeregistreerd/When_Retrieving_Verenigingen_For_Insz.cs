namespace AssociationRegistry.Test.Acm.Api.Given_FeitelijkeVerenigingWerdGeregistreerd;

using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using System.Net;
using templates;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AcmApiCollection))]
[Category("AcmApi")]
[IntegrationTest]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly HttpResponseMessage _response;
    private readonly FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario _scenario;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario;
        _response = fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz).GetAwaiter().GetResult();
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
                    _scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam,
                    Verenigingstype.VZER,
                    Verenigingssubtype.NietBepaald
                );

        content.Should().BeEquivalentJson(expected);
    }
}
