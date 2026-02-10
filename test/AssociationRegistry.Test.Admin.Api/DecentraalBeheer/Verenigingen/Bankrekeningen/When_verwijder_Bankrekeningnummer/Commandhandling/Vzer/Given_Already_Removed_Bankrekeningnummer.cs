namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Already_Removed_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdVerwijderdScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Already_Removed_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdVerwijderdScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerwijderBankrekeningnummerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_A_BankrekeningnummerIsNietGekend_Exception_Is_Thrown()
    {
        var command = new VerwijderBankrekeningnummerCommand(
            VCode: _scenario.VCode,
            BankrekeningnummerId: _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
        );

        var exception = await Assert.ThrowsAsync<BankrekeningnummerIsNietGekend>(async () =>
            await _commandHandler.Handle(
                new CommandEnvelope<VerwijderBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>())
            )
        );

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();

        exception
            .Message.Should()
            .Be(
                string.Format(
                    ExceptionMessages.BankrekeningnummerIsNietGekend,
                    _scenario.BankrekeningnummerWerdVerwijderd.BankrekeningnummerId
                )
            );
    }
}
