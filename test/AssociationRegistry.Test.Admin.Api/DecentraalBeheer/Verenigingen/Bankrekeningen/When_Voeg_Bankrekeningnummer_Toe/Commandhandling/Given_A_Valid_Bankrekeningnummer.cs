namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_A_Valid_Bankrekeningnummer
{
    private readonly VoegBankrekeningnummerToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Valid_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _commandHandler = new VoegBankrekeningnummerToeCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_BankrekeningWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _fixture.Create<VoegBankrekeningnummerToeCommand>() with
        {
            VCode = _scenario.VCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<VoegBankrekeningnummerToeCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdToegevoegd(
                1,
                command.Bankrekeningnummer.Iban.Value,
                command.Bankrekeningnummer.GebruiktVoor,
                command.Bankrekeningnummer.Titularis
            )
        );
    }
}
