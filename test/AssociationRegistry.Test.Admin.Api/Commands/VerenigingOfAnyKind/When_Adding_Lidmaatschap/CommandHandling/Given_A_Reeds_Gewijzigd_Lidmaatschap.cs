namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Lidmaatschap.CommandHandling;

using Acties.VoegLidmaatschapToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Reeds_Gewijzigd_Lidmaatschap
{
    [Fact]
    public async Task Then_It_Saves_A_Lidmaatschap_With_Correct_LidmaatschapId()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new LidmaatschapWerdGewijzigdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VoegLidmaatschapToeCommandHandler(verenigingRepositoryMock);

        var command = fixture.Create<VoegLidmaatschapToeCommand>() with
        {
            VCode = scenario.VCode,
        };

        await commandHandler.Handle(new CommandEnvelope<VoegLidmaatschapToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new LidmaatschapWerdToegevoegd(
                scenario.VCode,
                new Registratiedata.Lidmaatschap(
                2,
                command.Lidmaatschap.AndereVereniging,
                command.Lidmaatschap.AndereVerenigingNaam,
                command.Lidmaatschap.Geldigheidsperiode.Van.DateOnly,
                command.Lidmaatschap.Geldigheidsperiode.Tot.DateOnly,
                command.Lidmaatschap.Identificatie,
                command.Lidmaatschap.Beschrijving)));
    }
}
