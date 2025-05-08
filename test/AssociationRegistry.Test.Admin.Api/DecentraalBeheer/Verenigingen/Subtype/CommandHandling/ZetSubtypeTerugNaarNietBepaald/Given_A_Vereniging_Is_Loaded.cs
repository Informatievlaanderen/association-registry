namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.ZetSubtypeTerugNaarNietBepaald;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Moq;
using Xunit;

public class Given_A_Vereniging_Is_Loaded
{
    [Fact]
    public async ValueTask Verify_The_Correct_Vereniging_Is_Loaded()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new Mock<IVerenigingsRepository>();

        var commandHandler = new ZetSubtypeTerugNaarNietBepaaldCommandHandler(verenigingRepositoryMock.Object);

        var command = new ZetSubtypeTerugNaarNietBepaaldCommand(scenario.VCode);

        // we don't care if it throws, we want to check if the vereniging is correctly loaded
        await Assert.ThrowsAnyAsync<Exception>(() => commandHandler.Handle(new CommandEnvelope<ZetSubtypeTerugNaarNietBepaaldCommand>(command, fixture.Create<CommandMetadata>())));

        verenigingRepositoryMock.Verify(x => x.Load<Vereniging>(scenario.VCode, It.IsAny<CommandMetadata>(), false, false), Times.Once);
    }
}
