namespace AssociationRegistry.Test.Admin.Api.CommandHandler.Given_A_VerenigingWerdGeregistreerd.When_WijzigBasisgegevens;

using Events;
using AssociationRegistry.Framework;
using Vereniging.WijzigBasisgegevens;
using AutoFixture;
using Scenarios;
using Xunit;

public class With_A_Naam : IClassFixture<Given_A_Scenario_CommandHandlerFixture<VerenigingWerdGeregistreedScenario>>
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreedScenario _scenario;
    private const string NieuweNaam = "De nieuwe naam";

    public With_A_Naam(Given_A_Scenario_CommandHandlerFixture<VerenigingWerdGeregistreedScenario> classFixture)
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
