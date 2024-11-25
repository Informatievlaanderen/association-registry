namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Removing_A_Vereniging.CommandHandling;

using Acties.VerwijderVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework;
using Vereniging;
using Xunit;

public class With_A_Reden
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerwijderVerenigingCommand _command;

    public With_A_Reden()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        _command = new VerwijderVerenigingCommand(_scenario.VCode, Reden: "Omdat weg moet");

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new VerwijderVerenigingCommandHandler(_verenigingRepositoryMock);

        commandHandler.Handle(
            new CommandEnvelope<VerwijderVerenigingCommand>(_command, commandMetadata),
            CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_VerenigingWerdVerwijderd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingWerdVerwijderd(_scenario.VCode, Reden: "Omdat weg moet")
        );
    }
}
