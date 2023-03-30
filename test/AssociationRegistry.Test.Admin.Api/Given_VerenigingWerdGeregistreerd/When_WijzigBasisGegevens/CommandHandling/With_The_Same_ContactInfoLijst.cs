namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using EventStore;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using Moq;
using VCodes;
using Vereniging;
using Vereniging.WijzigBasisgegevens;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_The_Same_ContactInfoLijst
{
    private readonly VerenigingWerdGeregistreerdWithContactInfo_Commandhandler_ScenarioBase _scenarioBase;
    private readonly Mock<IVerenigingsRepository> _verenigingRepositoryMock;
    private readonly CommandMetadata _commandMetadata;
    private readonly CommandResult _result;

    public With_The_Same_ContactInfoLijst()
    {
        var scenarioFixture = new CommandHandlerScenarioFixture<VerenigingWerdGeregistreerdWithContactInfo_Commandhandler_ScenarioBase>();
        _scenarioBase = scenarioFixture.Scenario;

        _verenigingRepositoryMock = new Mock<IVerenigingsRepository>();

        var fixture = new Fixture().CustomizeAll();
        var command = new WijzigBasisgegevensCommand(
            _scenarioBase.VCode,
            ContactInfoLijst: _scenarioBase.ContactInfoLijst
                .Select(MapperExtensions.ToCommandDataType)
                .ToArray());

        _commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(new ClockStub(new DateTime(year: 2023, month: 3, day: 13)));

        _verenigingRepositoryMock
            .Setup(r => r.Load(_scenarioBase.VCode, _commandMetadata.ExpectedVersion))
            .ReturnsAsync(scenarioFixture.Vereniging);

        _verenigingRepositoryMock
            .Setup(r => r.Save(It.IsAny<Vereniging>(), It.IsAny<CommandMetadata>()))
            .ReturnsAsync(StreamActionResult.Empty);

        _result = commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, _commandMetadata),
            _verenigingRepositoryMock.Object).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.Verify(r => r.Load(VCode.Create(_scenarioBase.VCode), _commandMetadata.ExpectedVersion), Times.Once);
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _verenigingRepositoryMock.Verify(r => r.Save(It.Is<Vereniging>(v=>!v.UncommittedEvents.Any()), _commandMetadata), Times.Once);
    }

    [Fact]
    public void Then_The_Result_Should_Have_No_Changes()
    {
        _result.HasChanges().Should().BeFalse();
    }

    [Fact]
    public void Then_The_Resulting_Sequence_Is_Null()
    {
        _result.Sequence.Should().BeNull();
    }

    [Fact]
    public void Then_The_Resulting_Version_Is_Null()
    {
        _result.Version.Should().BeNull();
    }
}
