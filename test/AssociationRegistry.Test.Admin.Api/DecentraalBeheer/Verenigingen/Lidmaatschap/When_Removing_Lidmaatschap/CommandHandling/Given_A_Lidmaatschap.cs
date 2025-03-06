namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Removing_Lidmaatschap.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Lidmaatschappen.VerwijderLidmaatschap;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Lidmaatschap
{
    [Fact]
    public async Task Then_It_Saves_A_Lidmaatschap()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new LidmaatschapWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VerwijderLidmaatschapCommandHandler(verenigingRepositoryMock);


        var command = new VerwijderLidmaatschapCommand(scenario.VCode,
                                                       new LidmaatschapId(scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId));

        await commandHandler.Handle(new CommandEnvelope<VerwijderLidmaatschapCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new LidmaatschapWerdVerwijderd(
                scenario.VCode,
                scenario.LidmaatschapWerdToegevoegd.Lidmaatschap));
    }
}
