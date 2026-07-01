namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Geschorste_Erkenning
{
    public static IEnumerable<object[]> ScenariosWithErkenningId
    {
        get
        {
            var vzer = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario();
            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Status_Geschorst(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        await test.WithDefaultErkenningModification().WithInitiator(test.GetInitiatorOvoCode()).WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(
            hernieuwingsUrl: "https://example.org/renewal",
            status: ErkenningStatus.Geschorst.Value
        );
        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }
}
