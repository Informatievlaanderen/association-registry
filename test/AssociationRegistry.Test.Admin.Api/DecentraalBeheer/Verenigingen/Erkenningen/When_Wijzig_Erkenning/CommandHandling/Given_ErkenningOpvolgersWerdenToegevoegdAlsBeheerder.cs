namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;
using VmrScenario = Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid.ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario;
using VzerScenario = Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid.ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario;

public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
{
    public static IEnumerable<object[]> ScenariosWithErkenningId
    {
        get
        {
            var vzer = new VzerScenario();
            var vmr = new VmrScenario();
            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask Then_Saves_ErkenningWerdGewijzigd(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        await test.WithDefaultErkenningModification().WithInitiator(test.GetInitiatorOvoCode()).WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(hernieuwingsUrl: "https://example.org/renewal");
        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask Then_OrganisatieBevoegdheidService_Not_Called(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        await test.WithDefaultErkenningModification().WithInitiator(test.GetInitiatorOvoCode()).WhenHandled();

        test.OrganisatieBevoegdheidService.VerifyNever();
    }
}
