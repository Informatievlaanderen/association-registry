namespace AssociationRegistry.Test.Acm.Api.Given_A_Vereniging_Was_Synchronised;

using DecentraalBeheer.Vereniging;
using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;

[Collection(nameof(AcmApiCollection))]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly VerenigingMetRechtspersoonlijkheid_WithAllFields_EventsInDbScenario _scenario;
    private EventsInDbScenariosFixture _fixture;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _scenario = fixture.VerenigingMetRechtspersoonlijkheidWithAllFieldsEventsInDbScenario;
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
                    _scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId,
                    _scenario.NaamWerdGewijzigdInKbo.Naam,
                    Verenigingstype.Parse(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm),
                    null,
                    kboNummer: _scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer
                );

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async ValueTask Without_IncludeKboVereningen_Then_we_get_a_response_without_vereniging()
    {
        var response = _fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz, includeKboVerenigingen: false).GetAwaiter().GetResult();

        var content = await response.Content.ReadAsStringAsync();

        var expected =
            new VerenigingenPerInszResponseTemplate()
               .WithInsz(_scenario.Insz);

        content.Should().BeEquivalentJson(expected);
    }
}
