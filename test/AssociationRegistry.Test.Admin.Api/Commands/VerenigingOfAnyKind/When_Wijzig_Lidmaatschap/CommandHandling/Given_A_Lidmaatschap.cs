namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Lidmaatschap.CommandHandling;

using Acties.WijzigLidmaatschap;
using AssociationRegistry.Acties.VerwijderLidmaatschap;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Vereniging;
using AutoFixture;
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
                    command.Lidmaatschap.Geldigheidsperiode.Van.DateOnly,
                    command.Lidmaatschap.Geldigheidsperiode.Tot.DateOnly,
                    command.Lidmaatschap.Identificatie,
                    command.Lidmaatschap.Beschrijving)));
    }
}
