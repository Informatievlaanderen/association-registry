namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.
    When_verwijder_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Already_Removed_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdVerwijderdScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_Already_Removed_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdVerwijderdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _commandHandler = new VerwijderBankrekeningnummerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_BankrekeningnummerIsNietGekend_Exception_Is_Thrown()
    {
        var command = new VerwijderBankrekeningnummerCommand(VCode: _scenario.VCode,
                                                             BankrekeningnummerId: _scenario
                                                                .BankrekeningnummerWerdToegevoegd
                                                                .BankrekeningnummerId);

        var exception = await Assert.ThrowsAsync<BankrekeningnummerIsNietGekend>(
            async () => await _commandHandler.Handle(
                new CommandEnvelope<VerwijderBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>())));

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();

        exception.Message.Should().Be(string.Format(ExceptionMessages.BankrekeningnummerIsNietGekend, _scenario.BankrekeningnummerWerdVerwijderd.BankrekeningnummerId));
    }
}
