namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Basisgegevens.FeitelijkeVereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Same_UitgeschrevenVoorPubliekeDatastroom
{
    [Fact]
    public void True_Then_No_New_Event_Is_Saved()
    {
        var scenario = new VerborgenFeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new WijzigBasisgegevensCommand(scenario.VCode, IsUitgeschrevenUitPubliekeDatastroom: true);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            verenigingRepositoryMock,
            new ClockStub(fixture.Create<DateOnly>())).GetAwaiter().GetResult();

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }

    [Fact]
    public void False_Then_No_New_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new WijzigBasisgegevensCommand(scenario.VCode, IsUitgeschrevenUitPubliekeDatastroom: false);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            verenigingRepositoryMock,
            new ClockStub(fixture.Create<DateOnly>())).GetAwaiter().GetResult();

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
