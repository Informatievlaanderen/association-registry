namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
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

        await _commandHandler.Handle(
            new CommandEnvelope<VoegBankrekeningnummerToeCommand>(command, _fixture.Create<CommandMetadata>()));

        _aggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdToegevoegd(
                1,
                command.Bankrekeningnummer.Iban.Value,
                command.Bankrekeningnummer.Doel,
                command.Bankrekeningnummer.Titularis.Value
            )
        );
    }
}
