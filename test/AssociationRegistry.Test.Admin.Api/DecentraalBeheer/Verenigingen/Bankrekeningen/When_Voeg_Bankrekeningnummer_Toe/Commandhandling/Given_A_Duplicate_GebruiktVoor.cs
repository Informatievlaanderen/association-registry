namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_Duplicate_GebruiktVoor
{
    private readonly VoegBankrekeningnummerToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdToegevoegdScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Duplicate_GebruiktVoor()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdToegevoegdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _commandHandler = new VoegBankrekeningnummerToeCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_An_IbanMoetUniekZijn_Exception_Is_Thrown()
    {
        var command = _fixture.Create<VoegBankrekeningnummerToeCommand>() with
        {
            VCode = _scenario.VCode,
            Bankrekeningnummer = _fixture.Create<ToeTevoegenBankrekeningnummer>() with
            {
                GebruiktVoor = _scenario.BankrekeningnummerWerdToegevoegd.GebruiktVoor,
            }
        };

        var exception = await Assert.ThrowsAsync<GebruikVoorMoetUniekZijn>(async () =>  await _commandHandler.Handle(new CommandEnvelope<VoegBankrekeningnummerToeCommand>(command, _fixture.Create<CommandMetadata>())));

        exception.Message.Should().Be(ExceptionMessages.GebruiktVoorMoetUniekZijn);
    }
}
