namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_In_The_Future : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario>>
{
    private readonly CommandEnvelope<WijzigBasisgegevensCommand> _commandEnvelope;
    private readonly WijzigBasisgegevensCommandHandler _commandHandler;
    private readonly VerenigingRepositoryMock _repositoryMock;

    public With_A_Startdatum_In_The_Future(CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario> classFixture)
    {
        var fixture = new Fixture().CustomizeAll();
        _repositoryMock = classFixture.VerenigingRepositoryMock;
        var today = fixture.Create<DateOnly>();

        var command = fixture.Create<WijzigBasisgegevensCommand>() with
        {
            Startdatum = Startdatum.Create(today.AddDays(1)),
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
