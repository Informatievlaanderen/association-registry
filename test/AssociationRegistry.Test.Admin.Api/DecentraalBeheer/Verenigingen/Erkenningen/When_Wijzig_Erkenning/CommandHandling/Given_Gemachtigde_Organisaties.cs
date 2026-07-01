namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Gemachtigde_Organisaties
{
    public static IEnumerable<object[]> ScenariosWithErkenningId
    {
        get
        {
            var vzer = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningWerdGewijzigd(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var gemachtigdeOrganisatie = test.AggregateSessionMock.ToString();

        var service = test.OrganisatieBevoegdheidService.WithGemachtigdeOrganisaties(
            origineleErkenning.GeregistreerdDoor.OvoCode,
            [gemachtigdeOrganisatie]
        );

        await test.WithDefaultErkenningModification().WithInitiator(gemachtigdeOrganisatie).WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(hernieuwingsUrl: "https://example.org/renewal");
        var opvolgersEvent = new ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(
            erkenningId,
            new[] { gemachtigdeOrganisatie }
        );

        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent, opvolgersEvent);
    }
}
