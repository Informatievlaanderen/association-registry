namespace AssociationRegistry.Test.Admin.Api.Geotags.When_Herbereken_Geotags;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags.Messages;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Geotags;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Moq;

public class HerberekenGeotagsContext
{
    private readonly Fixture _fixture;
    private readonly HerberekenGeotagsCommandHandler _commandHandler;

    public CommandhandlerScenarioBase Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public Mock<IGeotagsService> GeotagsServiceMock { get; }
    public CommandMetadata Metadata { get; }

    public HerberekenGeotagsContext(CommandhandlerScenarioBase scenario)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        GeotagsServiceMock = new Mock<IGeotagsService>();
        _commandHandler = new HerberekenGeotagsCommandHandler(AggregateSessionMock, GeotagsServiceMock.Object);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public HerberekenGeotagsCommand CreateCommand() => new(Scenario.VCode!);

    public void SetupCalculateGeotags(GeotagsCollection geotags) =>
        GeotagsServiceMock
            .Setup(x => x.CalculateGeotags(It.IsAny<IEnumerable<Locatie>>(), It.IsAny<IEnumerable<Werkingsgebied>>()))
            .ReturnsAsync(geotags);

    public async ValueTask Handle(HerberekenGeotagsCommand? command = null, CommandMetadata? metadata = null) =>
        await _commandHandler.Handle(
            new CommandEnvelope<HerberekenGeotagsCommand>(command ?? CreateCommand(), metadata ?? Metadata)
        );
}
