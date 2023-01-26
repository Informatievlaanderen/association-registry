namespace AssociationRegistry.Test.Admin.Api.CommandHandler.Given_A_VerenigingWerdGeregistreerd.When_WijzigBasisgegevens;

using AssociationRegistry.Framework;
using Vereniging.WijzigBasisgegevens;
using AutoFixture;
using FluentAssertions;
using Scenarios;
using Vereniging;
using Xunit;

public class With_The_Same_Naam : IClassFixture<Given_A_Scenario_CommandHandlerFixture<VerenigingWerdGeregistreedScenario>>
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreedScenario _scenario;
    private CommandResult _result;

    public With_The_Same_Naam(Given_A_Scenario_CommandHandlerFixture<VerenigingWerdGeregistreedScenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _scenario = classFixture.Scenario;

        var fixture = new Fixture();
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, Naam: _scenario.Naam);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        _result = commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded(_scenario.VCode);
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }

    [Fact]
    public void Then_The_Result_Should_Have_No_Changes()
    {
        _result.HasChanges().Should().BeFalse();
    }
}
