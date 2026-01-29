namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.
    When_Valideer_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_OvoCode
{
    private readonly ValideerBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdToegevoegdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Invalid_OvoCode()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdToegevoegdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new ValideerBankrekeningnummerCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_Throws_BankrekeningnummerIsNietGekend()
    {
        var command = _fixture.Create<ValideerBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId = _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId + _fixture.Create<int>(),
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = CommandMetadata.VloOvoCode,
        };

        var exception = await Assert.ThrowsAsync<OvoCodeIsNietToegelatenDezeActieUitTeVoeren>(
            async () => await _commandHandler.Handle(
                new CommandEnvelope<ValideerBankrekeningnummerCommand>(command, commandMetadata)));

        exception.Message.Should().Be(string.Format(ExceptionMessages.OvoCodeIsNietGemachtigdOmDezeActieUitTeVoeren, CommandMetadata.VloOvoCode));
    }
}
