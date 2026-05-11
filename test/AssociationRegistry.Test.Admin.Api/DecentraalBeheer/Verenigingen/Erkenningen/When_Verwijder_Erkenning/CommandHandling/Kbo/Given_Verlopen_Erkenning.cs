namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verwijder_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerwijderErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Verlopen_Erkenning
{
    private readonly VerwijderErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Verlopen_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());
        _commandHandler = new VerwijderErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throw_VerlopenErkenningKanNietVerwijderdWorden()
    {
        var verlopenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<VerwijderErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = verlopenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        var commandEnvelope = new CommandEnvelope<VerwijderErkenningCommand>(command, commandMetadata);

        var exception = await Assert.ThrowsAsync<VerlopenErkenningKanNietVerwijderdWorden>(async () =>
        {
            await _commandHandler.Handle(commandEnvelope);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.VerlopenErkenningKanNietVerwijderdWorden));
    }
}
