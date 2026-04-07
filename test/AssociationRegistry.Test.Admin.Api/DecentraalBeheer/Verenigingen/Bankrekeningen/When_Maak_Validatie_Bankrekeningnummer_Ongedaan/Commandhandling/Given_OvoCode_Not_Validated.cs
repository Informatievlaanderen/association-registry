namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.
    When_Maak_Validatie_Bankrekeningnummer_Ongedaan.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.MaakValidatieBankrekeningOngedaan;
using FluentAssertions;
using Xunit;

public class Given_OvoCode_Not_Validated
{
    private readonly MaakValidatieBankrekeningnummerOngedaanCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdToegevoegdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_OvoCode_Not_Validated()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdToegevoegdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new MaakValidatieBankrekeningnummerOngedaanCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_Throws_ValidatieBankrekeningnummerIsNietGekend()
    {
        var command = _fixture.Create<MaakValidatieBankrekeningnummerOngedaanCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId =
            _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _fixture.Create<string>(),
        };

        var commandEnvelope =
            new CommandEnvelope<MaakValidatieBankrekeningnummerOngedaanCommand>(command, commandMetadata);

        var exception = await Assert.ThrowsAsync<ValidatieBankrekeningnummerIsNietGekend>(async () =>
        {
            await _commandHandler.Handle(commandEnvelope);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ValidatieBankrekeningnummerIsNietGekend,
                                                    commandMetadata.Initiator));
    }
}
