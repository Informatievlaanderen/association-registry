namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using AssociationRegistry.DecentraalBeheer.Basisgegevens.FeitelijkeVereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Moq;
using Vereniging.Geotags;

public static class WerkingsgebiedenScenarioRunner
{
    public static (VerenigingRepositoryMock verenigingRepositoryMock, GeotagsCollection geotags) Run(CommandhandlerScenarioBase scenario, Func<Fixture, Werkingsgebied[]> werkingsgebieden)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = new WijzigBasisgegevensCommand(scenario.VCode, Werkingsgebieden: werkingsgebieden(fixture));
        var commandMetadata = fixture.Create<CommandMetadata>();

        var (geotagsService, geotags) = Faktory.New().GeotagsService.ReturnsRandomGeotags();
        var commandHandler = new WijzigBasisgegevensCommandHandler(geotagsService.Object);

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            verenigingRepositoryMock,
            new ClockStub(fixture.Create<DateOnly>())).GetAwaiter().GetResult();

        return (verenigingRepositoryMock, geotags);
    }
}
