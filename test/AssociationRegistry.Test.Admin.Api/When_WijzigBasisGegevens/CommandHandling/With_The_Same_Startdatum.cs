namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using EventStore;
using AssociationRegistry.Framework;
using Fixtures;
using Fixtures.Scenarios;
using Vereniging;
using AutoFixture;
using Fakes;
using FluentAssertions;
using Framework;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_The_Same_Startdatum : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario>>
{
    private readonly Mock<IVerenigingsRepository> _verenigingRepositoryMock;
    private readonly CommandResult _result;
    private readonly CommandMetadata _commandMetadata;
    private readonly CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario> _classfixure;

    public With_The_Same_Startdatum(CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario> classFixture)
    {
        _verenigingRepositoryMock = new Mock<IVerenigingsRepository>();
        _classfixure = classFixture;

        var fixture = new Fixture().CustomizeAll();
        var command = new WijzigBasisgegevensCommand(_classfixure.Scenario.VCode, Startdatum: Startdatum.Create(_classfixure.Scenario.Startdatum));
        _commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        _verenigingRepositoryMock
            .Setup(r => r.Load(_classfixure.Scenario.VCode, _commandMetadata.ExpectedVersion))
            .ReturnsAsync(_classfixure.Vereniging);

        _verenigingRepositoryMock
            .Setup(r => r.Save(It.IsAny<Vereniging>(), It.IsAny<CommandMetadata>()))
            .ReturnsAsync(StreamActionResult.Empty);

        _result = commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, _commandMetadata),
            _verenigingRepositoryMock.Object,
            new ClockStub(fixture.Create<DateOnly>())).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.Verify(r => r.Load(VCode.Create(_classfixure.Scenario.VCode), _commandMetadata.ExpectedVersion), Times.Once);
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _verenigingRepositoryMock.Verify(r => r.Save(It.Is<Vereniging>(v => !v.UncommittedEvents.Any()), _commandMetadata), Times.Once);
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
