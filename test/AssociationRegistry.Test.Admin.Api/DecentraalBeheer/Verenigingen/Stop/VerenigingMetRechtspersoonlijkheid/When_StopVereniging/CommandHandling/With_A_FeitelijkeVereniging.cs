namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Stop.VerenigingMetRechtspersoonlijkheid.When_StopVereniging.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.StopVereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class With_A_VerenigingMetRechtspersoonlijkheid
{
    private readonly StopVerenigingCommandHandler _commandHandler;
    private readonly CommandEnvelope<StopVerenigingCommand> _envelope;

    public With_A_VerenigingMetRechtspersoonlijkheid()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        _commandHandler = new StopVerenigingCommandHandler(verenigingRepositoryMock, new ClockStub(fixture.Create<DateOnly>()));

        var command = new StopVerenigingCommand(scenario.VCode, fixture.Create<Datum>());
        var commandMetadata = fixture.Create<CommandMetadata>();

        _envelope = new CommandEnvelope<StopVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async ValueTask Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope, CancellationToken.None);
        await method.Should().ThrowAsync<VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden>();
    }
}
