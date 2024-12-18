namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using FluentAssertions;
using Framework.Fakes;
using Primitives;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_The_Same_Startdatum
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly CommandResult _result;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public With_The_Same_Startdatum()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new WijzigBasisgegevensCommand(_scenario.VCode,
                                                     Startdatum: NullOrEmpty<Datum>.Create(
                                                         Datum.Hydrate(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Startdatum)));

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        _result = commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock,
            new ClockStub(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Startdatum!.Value)).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldNotHaveSaved<StartdatumWerdGewijzigd>();
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
