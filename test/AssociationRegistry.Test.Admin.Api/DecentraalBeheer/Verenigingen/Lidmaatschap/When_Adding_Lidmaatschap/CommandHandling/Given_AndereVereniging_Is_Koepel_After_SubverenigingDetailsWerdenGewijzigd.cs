namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

/// <summary>
/// Changing identificatie or beschrijving of the subvereniging must not forget the koepel.
/// </summary>
public class Given_AndereVereniging_Is_Koepel_After_SubverenigingDetailsWerdenGewijzigd
{
    [Fact]
    public async ValueTask Then_It_Throws_VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();

        var state = scenario
            .GetVerenigingState()
            .Apply(
                new SubverenigingDetailsWerdenGewijzigd(
                    scenario.VCode,
                    fixture.Create<string>(),
                    fixture.Create<string>()
                )
            );

        var commandHandler = new VoegLidmaatschapToeCommandHandler(
            new AggregateSessionMock(state),
            new VerenigingsStateQueriesMock()
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
