namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_In_The_Future
{
    private readonly CommandEnvelope<WijzigBasisgegevensCommand> _commandEnvelope;
    private readonly WijzigBasisgegevensCommandHandler _commandHandler;
    private readonly VerenigingRepositoryMock _repositoryMock;

    public With_A_Startdatum_In_The_Future()
    {
        var fixture = new Fixture().CustomizeAll();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var command = fixture.Create<WijzigBasisgegevensCommand>() with
        {
            Startdatum = fixture.Create<Startdatum>(),
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new WijzigBasisgegevensCommandHandler();
        _commandEnvelope = new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_it_throws_an_StartdatumIsInFutureException()
    {
        var method = () => _commandHandler.Handle(
            _commandEnvelope,
            _repositoryMock,
            new ClockStub(_commandEnvelope.Command.Startdatum!.Datum!.Value.AddDays(-1)));
        await method.Should().ThrowAsync<StardatumIsInFuture>();
    }
}
