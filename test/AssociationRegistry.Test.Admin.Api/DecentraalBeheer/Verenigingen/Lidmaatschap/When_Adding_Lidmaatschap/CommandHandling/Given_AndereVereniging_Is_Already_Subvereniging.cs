namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Vereniging;
using Xunit;

public class Given_AndereVereniging_Is_Already_Subvereniging
{
    [Fact]
    public async ValueTask Then_It_Saves_A_Lidmaatschap()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();

        var verenigingRepositoryMock = new AggregateSessionMock(scenario.GetVerenigingState());

        var verenigingsStateQueriesMock = new VerenigingsStateQueriesMock();
        var commandHandler = new VoegLidmaatschapToeCommandHandler(
            verenigingRepositoryMock,
            verenigingsStateQueriesMock
        );

        var command = fixture.Create<VoegLidmaatschapToeCommand>() with
        {
            VCode = scenario.VCode,
            Lidmaatschap = fixture.Create<ToeTeVoegenLidmaatschap>() with
            {
                AndereVereniging = VCode.Create(
                    scenario.VerenigingssubtypeWerdVerfijndNaarSubvereniging.SubverenigingVan.AndereVereniging
                ),
            },
        };

        var exception = await Assert.ThrowsAnyAsync<VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs>(() =>
            commandHandler.Handle(
                new CommandEnvelope<VoegLidmaatschapToeCommand>(command, fixture.Create<CommandMetadata>())
            )
        );
        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs);
    }
}
