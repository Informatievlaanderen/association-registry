namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Maak_Subvereniging.CommandHandling;

using AssociationRegistry.Acties.VoegLidmaatschapToe;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Subvereniging
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
                new Registratiedata.Lidmaatschap(
                1,
                lidmaatschap.AndereVereniging,
                lidmaatschap.Geldigheidsperiode.Van.DateOnly,
                lidmaatschap.Geldigheidsperiode.Tot.DateOnly,
                lidmaatschap.Identificatie,
                lidmaatschap.Beschrijving)));
    }
}
