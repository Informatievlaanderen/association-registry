namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Lidmaatschap.CommandHandling;

using Acties.VoegLidmaatschapToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Lidmaatschap
{
    [Fact]
    public async Task Then_It_Saves_A_Lidmaatschap()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VoegLidmaatschapToeCommandHandler(verenigingRepositoryMock);

        var lidmaatschap = fixture.Create<Lidmaatschap>();

        var command = new VoegLidmaatschapToeCommand(scenario.VCode,
                                                     lidmaatschap);

        await commandHandler.Handle(new CommandEnvelope<VoegLidmaatschapToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new LidmaatschapWerdToegevoegd(
                scenario.VCode,
                new Registratiedata.Lidmaatschap(
                1,
                lidmaatschap.AndereVereniging,
                lidmaatschap.Geldigheidsperiode.Van.DateOnly,
                lidmaatschap.Geldigheidsperiode.Tot.DateOnly,
                lidmaatschap.Identificatie,
                lidmaatschap.Beschrijving)));
    }
}
