namespace AssociationRegistry.Test.Acm.Api.Given_VerengingWerdGestopt;

using Common.Extensions;
using DecentraalBeheer.Vereniging;
using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using System.Net;
using templates;
using Vereniging;
using Xunit;
using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;

[Collection(nameof(AcmApiCollection))]
public class When_retrieving_Vereniging_for_Insz
{
    private readonly HttpResponseMessage _response;
    private readonly FeitelijkeVerenigingWerdGestopt_EventsInDbScenario _scenario;

    public When_retrieving_Vereniging_for_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.FeitelijkeVerenigingWerdGestoptEventsInDbScenario;
        _response = fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_200()
        => _response.StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public async ValueTask Then_we_get_a_response_with_one_vereniging()
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
                    VerenigingssubtypeCode.Default,
                    VerenigingStatus.Gestopt
                );

        content.Should().BeEquivalentJson(expected);
    }
}
