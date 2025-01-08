namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using DecentraalBeheer.Basisgegevens.FeitelijkeVereniging;
using Events;
using FluentAssertions;
using Framework.Fakes;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_IngeschrevenVoorPubliekeDatastroom
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerborgenFeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public With_IngeschrevenVoorPubliekeDatastroom()
    {
        _scenario = new VerborgenFeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
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
    public void Then_A_VerenigingWerdToegevoegdAanPubliekeDatastroom_Event_Is_Saved()
    {
        _verenigingRepositoryMock
           .SaveInvocations[0]
           .Vereniging
           .UncommittedEvents
           .Should()
           .ContainSingle(e => e.GetType() == typeof(VerenigingWerdIngeschrevenInPubliekeDatastroom));
    }
}
