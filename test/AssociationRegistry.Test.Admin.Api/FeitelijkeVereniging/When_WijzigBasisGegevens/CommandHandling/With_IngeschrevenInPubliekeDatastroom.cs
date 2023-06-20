namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_IngeschrevenInPubliekeDatastroom
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerborgenFeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public With_IngeschrevenInPubliekeDatastroom()
    {
        _scenario = new VerborgenFeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAll();
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, IsUitgeschrevenUitPubliekeDatastroom: false);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock,
            new ClockStub(fixture.Create<DateOnly>())).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_VerenigingWerdIngeschrevenInPubliekeDatastroom_Event_Is_Saved()
    {
        _verenigingRepositoryMock
            .SaveInvocations[0]
            .Vereniging
            .UncommittedEvents
            .Should()
            .ContainSingle(e => e.GetType() == typeof(VerenigingWerdIngeschrevenInPubliekeDatastroom));
    }
}
