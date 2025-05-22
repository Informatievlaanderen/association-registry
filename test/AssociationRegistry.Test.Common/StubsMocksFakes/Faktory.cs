namespace AssociationRegistry.Test.Common.StubsMocksFakes;

using Admin.Api.Queries;
using AutoFixture;
using Clocks;
using global::AutoFixture;
using Moq;
using Stubs.VCodeServices;
using Vereniging;
using Vereniging.Geotags;
using VerenigingsRepositories;
using Wolverine;

public class Faktory(Fixture fixture)
{
    public static Faktory New(Fixture? fixture = null)
        => new(fixture ?? new Fixture().CustomizeDomain());

    public VCodeServiceFactory VCodeService { get; } = new VCodeServiceFactory();
    public VerenigingsRepositoryFactory VerenigingsRepository { get; } = new VerenigingsRepositoryFactory();
    public ClockFactory Clock { get; } = new ClockFactory();
    public GeotagsServiceFactory GeotagsService { get; } = new GeotagsServiceFactory(fixture);

    public VerenigingenZonderGeotagsQueryFactory VerenigingenZonderGeotagsQuery {get; } = new VerenigingenZonderGeotagsQueryFactory(fixture);
    public MessageBusFactory MessageBus { get; } = new MessageBusFactory(fixture);
}

public class MessageBusFactory
{
    public Mock<IMessageBus> Mock()
        => new();

    public MessageBusFactory(Fixture fixture)
    {
    }
}

public class VerenigingenZonderGeotagsQueryFactory
{
    public Mock<IVerenigingenWithoutGeotagsQuery> Mock(IEnumerable<string> returns)
    {
        var mock = new Mock<IVerenigingenWithoutGeotagsQuery>();
        mock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(returns.ToArray);
        return mock;
    }

    public VerenigingenZonderGeotagsQueryFactory(Fixture fixture)
    {
    }
}

public class VCodeServiceFactory
{
    public StubVCodeService Stub(VCode vCode)
        => new StubVCodeService(vCode);
}

public class VerenigingsRepositoryFactory
{
    public VerenigingRepositoryMock Mock()
        => new();
}

public class ClockFactory
{
    public ClockStub Stub(DateOnly date)
        => new(date);
}

public class GeotagsServiceFactory(Fixture fixture)
{
    public (IGeotagsService Object, Geotag[] geotags) MockWithRandomGeotags(
        IEnumerable<Locatie> locaties,
        IEnumerable<Werkingsgebied> werkingsgebieden)
    {
        var geotagsService = new Mock<IGeotagsService>();

        var geotags = fixture.CreateMany<Geotag>().ToArray();

        geotagsService.Setup(x => x.CalculateGeotags(locaties.ToArray(), werkingsgebieden))
                       .ReturnsAsync(geotags);

        return (geotagsService.Object, geotags);
    }

}
