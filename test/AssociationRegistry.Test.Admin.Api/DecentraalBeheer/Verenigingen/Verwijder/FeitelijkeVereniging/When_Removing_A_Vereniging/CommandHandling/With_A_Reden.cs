namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder.FeitelijkeVereniging.When_Removing_A_Vereniging.CommandHandling;

using AssociationRegistry.DecentraalBeheer.VerwijderVereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using System.Threading;
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
