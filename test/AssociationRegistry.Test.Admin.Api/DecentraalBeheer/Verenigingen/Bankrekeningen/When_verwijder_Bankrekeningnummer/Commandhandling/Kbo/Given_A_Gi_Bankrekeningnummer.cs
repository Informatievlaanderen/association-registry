namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling.Kbo;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_A_Gi_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersAddedByGIScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Gi_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersAddedByGIScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerwijderBankrekeningnummerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_BankrekeningWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = new VerwijderBankrekeningnummerCommand(
            VCode: _scenario.VCode,
            BankrekeningnummerId: _scenario.GIBankrekeningnummerWerdToegevoegd.BankrekeningnummerId
        );

        await _commandHandler.Handle(
            new CommandEnvelope<VerwijderBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdVerwijderd(
                command.BankrekeningnummerId,
                _scenario.GIBankrekeningnummerWerdToegevoegd.Iban
            )
        );
    }
}
