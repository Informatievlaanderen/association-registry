namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;
using FluentAssertions;
using Xunit;

public class Given_A_KBO_Vereniging
{
    private readonly VerwijderBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_KBO_Vereniging()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerwijderBankrekeningnummerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throws()
    {
        var command = _fixture.Create<VerwijderBankrekeningnummerCommand>() with { VCode = _scenario.VCode };

        var exception = await Assert.ThrowsAsync<ActieIsNietToegestaanVoorVerenigingstype>(async () =>
            await _commandHandler.Handle(
                new CommandEnvelope<VerwijderBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>())
            )
        );

        exception.Message.Should().Be(ExceptionMessages.UnsupportedOperationForVerenigingstype);
    }
}
