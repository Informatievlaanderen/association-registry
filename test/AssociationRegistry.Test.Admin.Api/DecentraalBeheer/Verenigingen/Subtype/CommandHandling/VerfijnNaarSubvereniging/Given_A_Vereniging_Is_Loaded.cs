namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using MartenDb.Store;
using Moq;
using Vereniging;
using Xunit;

public class Given_A_Vereniging_Is_Loaded
{
    [Fact]
    public async ValueTask Verify_The_Correct_Vereniging_Is_Loaded()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        var rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var aggregateSession = new Mock<IAggregateSession>();

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(aggregateSession.Object);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(
            scenario.VCode,
            new SubverenigingVanDto(rechtspersoonScenario.VCode, null, null)
        );

        // we don't care if it throws, we want to check if the vereniging is correctly loaded
        await Assert.ThrowsAnyAsync<Exception>(() =>
            commandHandler.Handle(
                new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>())
            )
        );

        aggregateSession.Verify(
            x => x.Load<Vereniging>(scenario.VCode, It.IsAny<CommandMetadata>(), false, false),
            Times.Once
        );
        aggregateSession.Verify(
            x =>
                x.Load<VerenigingMetRechtspersoonlijkheid>(
                    rechtspersoonScenario.VCode,
                    It.IsAny<CommandMetadata>(),
                    false,
                    false
                ),
            Times.Once
        );
    }
}
