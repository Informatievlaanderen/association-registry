namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.Basisgegevens.FeitelijkeVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Framework.Fakes;
using Primitives;
using Vereniging;
using Vereniging.Exceptions;
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
        var fixture = new Fixture().CustomizeAdminApi();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var command = fixture.Create<WijzigBasisgegevensCommand>() with
        {
            Startdatum = NullOrEmpty<Datum>.Create(fixture.Create<Datum>()),
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
            new ClockStub(_commandEnvelope.Command.Startdatum.Value!.Value.AddDays(-1)));

        await method.Should().ThrowAsync<StartdatumMagNietInToekomstZijn>();
    }
}
