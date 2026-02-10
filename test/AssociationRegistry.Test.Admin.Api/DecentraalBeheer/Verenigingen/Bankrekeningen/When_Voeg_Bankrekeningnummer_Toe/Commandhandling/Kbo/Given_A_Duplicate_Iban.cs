namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_Duplicate_Iban
{
    private readonly VoegBankrekeningnummerToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_Duplicate_Iban()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VoegBankrekeningnummerToeCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_Throws()
    {
        var command = _fixture.Create<VoegBankrekeningnummerToeCommand>() with
        {
            VCode = _scenario.VCode,
            Bankrekeningnummer = _fixture.Create<ToeTevoegenBankrekeningnummer>() with
            {
                Iban = IbanNummer.Hydrate(_scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.Iban),
            },
        };

        var exception = await Assert.ThrowsAsync<IbanMoetUniekZijn>(async () =>
            await _commandHandler.Handle(
                new CommandEnvelope<VoegBankrekeningnummerToeCommand>(command, _fixture.Create<CommandMetadata>())
            )
        );

        exception.Message.Should().Be(ExceptionMessages.IbanMoetUniekZijn);
    }
}
