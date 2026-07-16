namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Xunit;

public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
{
    public static IEnumerable<object[]> ScenariosWithErkenningId
    {
        get
        {
            var vzer = new ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario();
            var vmr = new ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario();
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
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        (
            await test.WithCommand(command => command.CreateRandomizedForSameVCodeAndErkenningId(test.Fixture))
                .WithInitiator(test.GetInitiatorOvoCode())
                .WhenHandled()
        ).ShouldHaveSavedErkenningWerdGewijzigd(test.Command.MapToExpectedEvent());
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask Then_OrganisatieBevoegdheidService_Not_Called(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        await test.WithCommand(command =>
                test.Fixture.Create<WijzigErkenningCommand>() with
                {
                    Erkenning = test.Fixture.Create<TeWijzigenErkenning>() with
                    {
                        ErkenningId = command.Erkenning.ErkenningId,
                    },
                    VCode = command.VCode,
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        test.OrganisatieBevoegdheidService.VerifyNever();
    }
}
