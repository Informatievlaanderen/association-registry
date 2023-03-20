namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Framework;
using Primitives;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Vereniging.WijzigBasisgegevens;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_Null : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario>>
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreerd_Commandhandler_Scenario _scenario;
    private readonly DateOnly _nieuweStartdatum;

    public With_A_Startdatum_Null(CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;

        var fixture = new Fixture();
        _scenario = classFixture.Scenario;
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, Startdatum: NullOrEmpty<DateOnly>.Null);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(new ClockStub(new DateTime(2023, 3, 6)));

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_A_StartdatumWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
