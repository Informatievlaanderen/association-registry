namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_KBO_Vereniging
{
    private readonly VoegBankrekeningnummerToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_KBO_Vereniging()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VoegBankrekeningnummerToeCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_Throws()
    {
        var command = _fixture.Create<VoegBankrekeningnummerToeCommand>() with { VCode = _scenario.VCode };

        var exception = await Assert.ThrowsAsync<ActieIsNietToegestaanVoorVerenigingstype>(async () =>
            await _commandHandler.Handle(
                new CommandEnvelope<VoegBankrekeningnummerToeCommand>(command, _fixture.Create<CommandMetadata>())
            )
        );

        exception.Message.Should().Be(ExceptionMessages.UnsupportedOperationForVerenigingstype);
    }
}
