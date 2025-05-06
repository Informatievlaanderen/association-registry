namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Moq;
using Vereniging;
using Xunit;

public class Given_A_Vereniging_Is_Loaded
{
    [Fact]
    public async Task Verify_The_Correct_Vereniging_Is_Loaded()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        var rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new Mock<IVerenigingsRepository>();

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(verenigingRepositoryMock.Object);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(scenario.VCode, new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(rechtspersoonScenario.VCode, null, null));

        // we don't care if it throws, we want to check if the vereniging is correctly loaded
        await Assert.ThrowsAnyAsync<Exception>(() => commandHandler.Handle(
                                                   new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, fixture.Create<CommandMetadata>())));

        verenigingRepositoryMock.Verify(x => x.Load<Vereniging>(scenario.VCode, It.IsAny<CommandMetadata>(), false, false), Times.Once);
        verenigingRepositoryMock.Verify(x => x.Load<VerenigingMetRechtspersoonlijkheid>(rechtspersoonScenario.VCode, It.IsAny<CommandMetadata>(), false, false), Times.Once);
    }
}
