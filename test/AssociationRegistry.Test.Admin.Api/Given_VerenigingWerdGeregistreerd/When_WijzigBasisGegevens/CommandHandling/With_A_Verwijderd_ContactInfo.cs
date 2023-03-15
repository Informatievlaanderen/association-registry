namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Framework;
using Vereniging.WijzigBasisgegevens;
using Xunit;

public class With_A_Verwijderd_ContactInfo
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreerdWithContactInfo_Commandhandler_Scenario _scenario;

    public With_A_Verwijderd_ContactInfo()
    {
        var scenarioFixture = new CommandHandlerScenarioFixture<VerenigingWerdGeregistreerdWithContactInfo_Commandhandler_Scenario>();
        _scenario = scenarioFixture.Scenario;

        _verenigingRepositoryMock = scenarioFixture.VerenigingRepositoryMock;

        var fixture = new Fixture().CustomizeAll();
        var command = new WijzigBasisgegevensCommand(
            VCode: _scenario.VCode,
            ContactInfoLijst: _scenario.ContactInfoLijst
                .Skip(1)
                .Select(MapperExtensions.ToCommandDataType)
                .ToArray());
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(new ClockStub(new DateTime(2023, 3, 13)));

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
    public void Then_A_ContactInfoLijstWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactInfoLijstWerdGewijzigd(
                _scenario.VCode,
                Array.Empty<Events.CommonEventDataTypes.ContactInfo>(),
                new[]
                {
                    _scenario.ContactInfoLijst.First(),
                },
                Array.Empty<Events.CommonEventDataTypes.ContactInfo>()
            ));
    }
}
