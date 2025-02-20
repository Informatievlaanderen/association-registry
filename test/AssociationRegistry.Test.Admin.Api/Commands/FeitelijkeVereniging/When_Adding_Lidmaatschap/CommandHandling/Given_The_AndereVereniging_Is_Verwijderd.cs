namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Lidmaatschap.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using DecentraalBeheer.Lidmaatschappen.VoegLidmaatschapToe;
using Moq;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_The_AndereVereniging_Is_Verwijderd
{
    [Fact]
    public async Task Then_It_Saves_A_Lidmaatschap()
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
