namespace AssociationRegistry.Test.Acm.Api.Given_RechtsvormWerdGewijzigdInKBO;

using DecentraalBeheer.Vereniging;
using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using System.Net;
using templates;
using Vereniging;
using Xunit;

[Collection(nameof(AcmApiCollection))]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly RechtsvormWerdGewijzigdInKBO_EventsInDbScenario _scenario;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _scenario = fixture.RechtsvormWerdGewijzigdInKBOEventsInDbScenario;
    }

    [Fact]
    public async ValueTask With_IncludeKboVereningen_Then_we_get_a_response_with_one_vereniging()
    {
        var response = _fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz, includeKboVerenigingen: true).GetAwaiter().GetResult();

        var content = await response.Content.ReadAsStringAsync();

        var expected =
            new VerenigingenPerInszResponseTemplate()
               .WithInsz(_scenario.Insz)
               .WithVereniging(
                    _scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                    _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    _scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
                    Verenigingstype.Parse(_scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm),
                    null,
                    kboNummer: _scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer
                );

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async ValueTask Without_IncludeKboVereningen_Then_we_get_a_response_with_one_vereniging()
    {
        var response = _fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz, includeKboVerenigingen: false).GetAwaiter().GetResult();

        var content = await response.Content.ReadAsStringAsync();

        var expected =
            new VerenigingenPerInszResponseTemplate()
               .WithInsz(_scenario.Insz);

        content.Should().BeEquivalentJson(expected);
    }
}
