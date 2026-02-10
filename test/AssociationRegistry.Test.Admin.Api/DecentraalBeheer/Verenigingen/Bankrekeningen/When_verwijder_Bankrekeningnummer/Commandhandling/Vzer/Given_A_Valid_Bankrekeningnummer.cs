namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Xunit;

public class Given_A_Valid_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdToegevoegdScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Valid_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdToegevoegdScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerwijderBankrekeningnummerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_BankrekeningWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = new VerwijderBankrekeningnummerCommand(
            VCode: _scenario.VCode,
            BankrekeningnummerId: _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
        );

        await _commandHandler.Handle(
            new CommandEnvelope<VerwijderBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdVerwijderd(
                command.BankrekeningnummerId,
                _scenario.BankrekeningnummerWerdToegevoegd.Iban
            )
        );
    }
}
