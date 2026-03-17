namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Maak_Validatie_Bankrekeningnummer_Ongedaan.Commandhandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.MaakValidatieBankrekeningOngedaan;
using FluentAssertions;
using Xunit;

public class Given_Unknown_Bankrekeningnummer
{
    private readonly MaakValidatieBankrekeningnummerOngedaanCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdToegevoegdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Unknown_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdToegevoegdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new MaakValidatieBankrekeningnummerOngedaanCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_Throws_BankrekeningnummerIsNietGekend()
    {
        var command = _fixture.Create<MaakValidatieBankrekeningnummerOngedaanCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId = _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId + _fixture.Create<int>(),
        };

        var exception = await Assert.ThrowsAsync<BankrekeningnummerIsNietGekend>(
            async () => await _commandHandler.Handle(
                new CommandEnvelope<MaakValidatieBankrekeningnummerOngedaanCommand>(command, _fixture.Create<CommandMetadata>())));

        exception.Message.Should().Be(string.Format(ExceptionMessages.BankrekeningnummerIsNietGekend, command.BankrekeningnummerId));
    }
}
