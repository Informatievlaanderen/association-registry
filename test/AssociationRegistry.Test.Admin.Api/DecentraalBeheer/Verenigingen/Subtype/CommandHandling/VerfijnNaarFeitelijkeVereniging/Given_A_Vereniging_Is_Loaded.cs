namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarFeitelijkeVereniging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
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

        var aggregateSessionMock = new Mock<IAggregateSession>();

        var commandHandler = new VerfijnSubtypeNaarFeitelijkeVerenigingCommandHandler(aggregateSessionMock.Object);

        var command = new VerfijnSubtypeNaarFeitelijkeVerenigingCommand(scenario.VCode);

        // we don't care if it throws, we want to check if the vereniging is correctly loaded
        await Assert.ThrowsAnyAsync<Exception>(() =>
            commandHandler.Handle(
                new CommandEnvelope<VerfijnSubtypeNaarFeitelijkeVerenigingCommand>(
                    command,
                    fixture.Create<CommandMetadata>()
                )
            )
        );

        aggregateSessionMock.Verify(
            x => x.Load<Vereniging>(scenario.VCode, It.IsAny<CommandMetadata>(), false, false),
            Times.Once
        );
    }
}
