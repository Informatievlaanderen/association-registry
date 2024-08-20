namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_StopVereniging.CommandHandling;

using AssociationRegistry.Acties.StopVereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Framework.Fakes;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
    public async Task Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope, CancellationToken.None);
        await method.Should().ThrowAsync<VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden>();
    }
}
