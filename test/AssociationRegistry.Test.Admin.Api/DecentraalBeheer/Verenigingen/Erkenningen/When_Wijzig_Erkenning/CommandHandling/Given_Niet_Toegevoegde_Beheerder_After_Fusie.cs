namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Niet_Toegevoegde_Beheerder_After_Fusie
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
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var nietToegevoegdeBeheerderAfterFusie = test.Metadata.Initiator;

        test
           .WithCommand(command => command.CreateRandomizedForSameVCodeAndErkenningId(test.Fixture))
           .WithInitiator(nietToegevoegdeBeheerderAfterFusie);

        var initiatorOvoCode = test.GetInitiatorOvoCode();

        test.OrganisatieBevoegdheidService.WithGemachtigdeOrganisaties(
            initiatorOvoCode,
            [nietToegevoegdeBeheerderAfterFusie, initiatorOvoCode]
        );

        await test.WhenHandled();

        var opvolgersEvent = new ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(
            erkenningId,
            [nietToegevoegdeBeheerderAfterFusie, initiatorOvoCode]
        );

        test.ShouldHaveSavedErkenningWerdGewijzigd(test.Command.MapToExpectedEvent(), opvolgersEvent);
    }
}
