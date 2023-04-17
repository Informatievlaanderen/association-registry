namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using AutoFixture;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_Null : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario>>
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_A_Startdatum_Null(CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;

        var fixture = new Fixture().CustomizeAll();
        var command = new WijzigBasisgegevensCommand(classFixture.Scenario.VCode, Startdatum: null);
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
