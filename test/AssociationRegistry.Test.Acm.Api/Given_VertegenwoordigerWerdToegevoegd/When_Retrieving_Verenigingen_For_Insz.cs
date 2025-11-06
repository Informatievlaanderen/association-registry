namespace AssociationRegistry.Test.Acm.Api.Given_VertegenwoordigerWerdToegevoegd;

using DecentraalBeheer.Vereniging;
using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using Marten;
using System.Net;
using templates;
using Vereniging;
using Xunit;

[Collection(nameof(AcmApiCollection))]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly HttpResponseMessage _response;
    private readonly VertegenwoordigerWerdToegevoegd_EventsInDbScenario _scenario;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.VertegenwoordigerWerdToegevoegdEventsInDbScenario;
        _response = fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_200()
        => _response.StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public async ValueTask Then_we_get_a_response_with_one_vereniging()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected = new VerenigingenPerInszResponseTemplate()
                      .WithInsz(_scenario.Insz)
                      .WithVereniging(
                           _scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
                           _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                           _scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam,
                           Verenigingstype.VZER,
                           VerenigingssubtypeCode.Default
                       );

        content.Should().BeEquivalentJson(expected);
    }
}
