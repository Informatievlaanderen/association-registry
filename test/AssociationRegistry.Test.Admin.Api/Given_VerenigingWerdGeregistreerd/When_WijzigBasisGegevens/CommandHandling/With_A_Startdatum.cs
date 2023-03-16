namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Primitives;
using Vereniging.WijzigBasisgegevens;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario>>
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreerd_Commandhandler_Scenario _scenario;
    private readonly DateOnly _nieuweStartdatum;

    public With_A_Startdatum(CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;

        var fixture = new Fixture();
        _scenario = classFixture.Scenario;
        _nieuweStartdatum = _scenario.Startdatum.AddDays(-1);
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, Startdatum: NullOrEmpty<DateOnly>.Create(_nieuweStartdatum));
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(new ClockStub(new DateTime(2023, 3, 6)));

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded(_scenario.VCode);
    }

    [Fact]
    public void Then_A_StartdatumWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new StartdatumWerdGewijzigd(_scenario.VCode, _nieuweStartdatum)
        );
    }
}
