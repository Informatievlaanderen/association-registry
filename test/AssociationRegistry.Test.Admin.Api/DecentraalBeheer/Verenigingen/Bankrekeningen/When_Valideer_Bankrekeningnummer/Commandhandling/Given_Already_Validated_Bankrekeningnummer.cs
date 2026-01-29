namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class Given_Already_Validated_Bankrekeningnummer
{
    private readonly ValideerBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdGevalideerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Already_Validated_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdGevalideerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new ValideerBankrekeningnummerCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_Nothing()
    {
        var command = _fixture.Create<ValideerBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId = _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
        };

        await _commandHandler.Handle(new CommandEnvelope<ValideerBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>()));

        _aggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
