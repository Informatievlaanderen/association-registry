namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.Handling_The_Command;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging.WijzigBasisgegevens;
using AutoFixture;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Naam : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreed_Commandhandler_Scenario>>
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreed_Commandhandler_Scenario _scenario;
    private const string NieuweNaam = "De nieuwe naam";

    public With_A_Naam(CommandHandlerScenarioFixture<VerenigingWerdGeregistreed_Commandhandler_Scenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;

        var fixture = new Fixture();
        _scenario = classFixture.Scenario;
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, Naam: NieuweNaam);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

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
    public void Then_A_NaamWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new NaamWerdGewijzigd(_scenario.VCode, NieuweNaam)
        );
    }
}
