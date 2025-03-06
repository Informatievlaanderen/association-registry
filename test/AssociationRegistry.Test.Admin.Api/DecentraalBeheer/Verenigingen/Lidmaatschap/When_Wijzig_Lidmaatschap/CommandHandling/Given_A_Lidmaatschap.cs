namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Wijzig_Lidmaatschap.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Lidmaatschappen.WijzigLidmaatschap;
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

        var commandHandler = new WijzigLidmaatschapCommandHandler(verenigingRepositoryMock);

        var command = fixture.Create<WijzigLidmaatschapCommand>() with
        {
            VCode = scenario.VCode,
            Lidmaatschap = fixture.Create<WijzigLidmaatschapCommand.TeWijzigenLidmaatschap>() with
            {
                LidmaatschapId = new LidmaatschapId(scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId),
            },
        };

        await commandHandler.Handle(new CommandEnvelope<WijzigLidmaatschapCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new LidmaatschapWerdGewijzigd(
                scenario.VCode,
                new Registratiedata.Lidmaatschap(
                    command.Lidmaatschap.LidmaatschapId,
                    scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
                    scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVerenigingNaam,
                    command.Lidmaatschap.GeldigVan,
                    command.Lidmaatschap.GeldigTot,
                    command.Lidmaatschap.Identificatie,
                    command.Lidmaatschap.Beschrijving)));
    }
}
