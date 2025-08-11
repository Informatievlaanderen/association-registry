namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Moq;
using Xunit;

public class Given_The_AndereVereniging_Is_Verwijderd
{
    [Fact]
    public async ValueTask Then_It_Saves_A_Lidmaatschap()
    {
        var fixture = new Fixture().CustomizeDomain();
        var repositoryMock = new Mock<IVerenigingsRepository>();

        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var command = fixture.Create<VoegLidmaatschapToeCommand>() with
        {
            VCode = scenario.VCode,
        };
        repositoryMock
           .Setup(x => x.IsVerwijderd(command.Lidmaatschap.AndereVereniging))
           .ReturnsAsync(true);

        var commandHandler = new VoegLidmaatschapToeCommandHandler(repositoryMock.Object);

        await Assert.ThrowsAsync<VerenigingKanGeenLidWordenVanVerwijderdeVereniging>(
            async () => await commandHandler.Handle(
                new CommandEnvelope<VoegLidmaatschapToeCommand>(command, fixture.Create<CommandMetadata>())));
    }
}
