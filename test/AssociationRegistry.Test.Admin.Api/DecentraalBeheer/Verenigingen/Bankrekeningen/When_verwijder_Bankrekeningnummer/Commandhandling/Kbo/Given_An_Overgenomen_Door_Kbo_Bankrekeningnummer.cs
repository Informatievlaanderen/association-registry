namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.
    When_verwijder_Bankrekeningnummer.Commandhandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_An_Overgenomen_Door_Kbo_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdOvergenomenVanuitKBOScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_An_Overgenomen_Door_Kbo_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdOvergenomenVanuitKBOScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerwijderBankrekeningnummerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throws()
    {
        var command = _fixture.Create<VerwijderBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId = _scenario.BankrekeningnummerWerdOvergenomenVanuitKBO.BankrekeningnummerId
        };

        var exception = await Assert.ThrowsAsync<ActieIsNietToegestaanVoorKboBankrekeningnummer>(async () =>
            await _commandHandler.Handle(
                new CommandEnvelope<VerwijderBankrekeningnummerCommand>(
                    command,
                    _fixture.Create<CommandMetadata>())
            )
        );

        exception.Message.Should().Be(ExceptionMessages.UnsupportedOperationForKboBankrekeningnummer);
    }
}
