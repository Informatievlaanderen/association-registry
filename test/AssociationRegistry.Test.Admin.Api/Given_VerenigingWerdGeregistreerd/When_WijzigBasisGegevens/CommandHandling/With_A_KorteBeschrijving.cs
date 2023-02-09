namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;
using AssociationRegistry.Vereniging.WijzigBasisgegevens;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_KorteBeschrijving : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreed_Commandhandler_Scenario>>
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreed_Commandhandler_Scenario _scenario;
    private const string NieuweKorteBeschrijving = "Een nieuwe beschrijving van de vereniging";

    public With_A_KorteBeschrijving(CommandHandlerScenarioFixture<VerenigingWerdGeregistreed_Commandhandler_Scenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _scenario = classFixture.Scenario;

        var fixture = new Fixture();
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, KorteBeschrijving: NieuweKorteBeschrijving);
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
            new KorteBeschrijvingWerdGewijzigd(_scenario.VCode, NieuweKorteBeschrijving)
        );
    }
}
