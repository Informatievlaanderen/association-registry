namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.
    CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using FluentAssertions;
using Xunit;

public class Given_Geschorste_Erkenning
{
    // TODO implement

    // private readonly ActiveerErkenningCommandHandler _commandHandler;
    // private readonly Fixture _fixture;
    // private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    // private readonly AggregateSessionMock _verenigingRepositoryMock;
    //
    // public Given_Startdatum_In_Future()
    // {
    //     _fixture = new Fixture().CustomizeAdminApi();
    //
    //     _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
    //     _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());
    //
    //     _commandHandler = new ActiveerErkenningCommandHandler(_verenigingRepositoryMock);
    // }
    //
    // [Fact]
    // public async ValueTask Then_No_Saved_Event()
    // {
    //     var teActiverenErkenningId = _scenario.ErkenningWerdGeregistreerdInToekomst.ErkenningId;
    //
    //     var command = _fixture.Create<ActiveerErkenningCommand>() with
    //     {
    //         VCode = _scenario.VCode,
    //         ErkenningId = teActiverenErkenningId,
    //     };
    //
    //     var commandMetadata = _fixture.Create<CommandMetadata>() with
    //     {
    //         Initiator = _scenario.ErkenningWerdGeregistreerdInToekomst.GeregistreerdDoor.OvoCode,
    //     };
    //
    //     var exception = await Assert.ThrowsAsync<ErkenningKanNietGeactiveerdWorden>(async () =>
    //             await _commandHandler.Handle(
    //                 new CommandEnvelope<ActiveerErkenningCommand>(command, commandMetadata))
    //         );
    //
    //     _verenigingRepositoryMock.ShouldNotHaveAnySaves(); exception
    //                                                       .Message.Should()
    //                                                       .Be(
    //                                                            string.Format(
    //                                                                "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet geactiveerd worden.",
    //                                                                _scenario.ErkenningWerdGeregistreerdInToekomst.ErkenningId,
    //                                                                _scenario.ErkenningWerdGeregistreerdInToekomst.Startdatum.Value,
    //                                                                _scenario.ErkenningWerdGeregistreerdInToekomst.Einddatum.Value,
    //                                                                ErkenningStatus.InAanvraagValue
    //                                                            )
    //                                                        );
    // }
}
