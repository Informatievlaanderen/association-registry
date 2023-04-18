namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using Fakes;
using Fixtures.Scenarios;
using AutoFixture;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_Null
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_A_Startdatum_Null()
    {
        var scenario = new VerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVereniging());

        var fixture = new Fixture().CustomizeAll();
        var command = new WijzigBasisgegevensCommand(scenario.VCode, Startdatum: null);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock, new ClockStub(fixture.Create<DateOnly>())).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_A_StartdatumWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
