namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Another_OvoCode
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
    public async ValueTask Then_It_Saves_An_ErkenningWerdGeschorst_Event(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        WijzigErkenningContext<CommandhandlerScenarioBase> tempQualifier =
            WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);
        var hernieuwingsUrl = tempQualifier.Fixture.Create<HernieuwingsUrl>().Value;
        tempQualifier.WithErkenningCommand(command =>
            tempQualifier.Command with
            {
                Erkenning = tempQualifier.Command.Erkenning with { HernieuwingsUrl = hernieuwingsUrl },
            }
        );

        var test = tempQualifier.WithInitiator("other-ovo-code");

        var exception = await Assert.ThrowsAsync<GiIsNietBevoegd>(async () => await test.WhenHandled());

        exception.Message.Should().Be(ExceptionMessages.GiIsNietBevoegd);
    }
}
