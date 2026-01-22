namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class Given_A_Reeds_Gewijzigd_Lidmaatschap
{
    [Fact]
    public async ValueTask Then_It_Saves_A_Lidmaatschap_With_Correct_LidmaatschapId()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new LidmaatschapWerdGewijzigdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var verenigingsStateQueriesMock = new VerenigingsStateQueriesMock();
        var commandHandler = new VoegLidmaatschapToeCommandHandler(
            verenigingRepositoryMock,
            verenigingsStateQueriesMock
        );

        var command = fixture.Create<VoegLidmaatschapToeCommand>() with { VCode = scenario.VCode };

        await commandHandler.Handle(
            new CommandEnvelope<VoegLidmaatschapToeCommand>(command, fixture.Create<CommandMetadata>())
        );

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new LidmaatschapWerdToegevoegd(
                scenario.VCode,
                new Registratiedata.Lidmaatschap(
                    2,
                    command.Lidmaatschap.AndereVereniging,
                    command.Lidmaatschap.AndereVerenigingNaam,
                    command.Lidmaatschap.Geldigheidsperiode.Van.DateOnly,
                    command.Lidmaatschap.Geldigheidsperiode.Tot.DateOnly,
                    command.Lidmaatschap.Identificatie,
                    command.Lidmaatschap.Beschrijving
                )
            )
        );
    }
}
