namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Lidmaatschap.CommandHandling;

using Acties.VoegLidmaatschapToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
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
        var lidmaatschap = fixture.Create<Lidmaatschap>();

        repositoryMock
           .Setup(x => x.IsVerwijderd(lidmaatschap.AndereVereniging))
           .ReturnsAsync(true);

        var commandHandler = new VoegLidmaatschapToeCommandHandler(repositoryMock.Object);

        var command = new VoegLidmaatschapToeCommand(scenario.VCode,
                                                     lidmaatschap);

        await Assert.ThrowsAsync<VerenigingKanGeenLidWordenVanVerwijderdeVereniging>(
            async () => await commandHandler.Handle(
                new CommandEnvelope<VoegLidmaatschapToeCommand>(command, fixture.Create<CommandMetadata>())));
    }
}
