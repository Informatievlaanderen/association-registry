namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_SyncKbo.CommandHandling;

using Acties.SyncKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Kbo;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_FeitelijkeVereniging
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly SyncKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<SyncKboCommand> _envelope;

    public With_A_FeitelijkeVereniging()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new SyncKboCommand(scenario.VCode, VerenigingVolgensKbo: fixture.Create<VerenigingVolgensKbo>());
        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new SyncKboCommandHandler();
        _envelope = new CommandEnvelope<SyncKboCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope, _verenigingRepositoryMock);
        await method.Should().ThrowAsync<ActieIsNietToegestaanVoorVerenigingstype>();
    }
}
