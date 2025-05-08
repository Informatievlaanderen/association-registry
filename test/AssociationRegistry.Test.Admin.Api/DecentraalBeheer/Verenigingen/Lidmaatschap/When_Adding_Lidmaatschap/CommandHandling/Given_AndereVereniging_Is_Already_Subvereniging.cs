namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_AndereVereniging_Is_Already_Subvereniging
{
    [Fact]
    public async ValueTask Then_It_Saves_A_Lidmaatschap()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VoegLidmaatschapToeCommandHandler(verenigingRepositoryMock);

        var command = fixture.Create<VoegLidmaatschapToeCommand>() with
        {
            VCode = scenario.VCode,
            Lidmaatschap = fixture.Create<VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap>() with
            {
                AndereVereniging = VCode.Create(scenario.VerenigingssubtypeWerdVerfijndNaarSubvereniging.SubverenigingVan.AndereVereniging),
            },
        };

        var exception = await Assert.ThrowsAnyAsync<VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs>(() => commandHandler.Handle(new CommandEnvelope<VoegLidmaatschapToeCommand>(command, fixture.Create<CommandMetadata>())));
        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs);
    }
}
