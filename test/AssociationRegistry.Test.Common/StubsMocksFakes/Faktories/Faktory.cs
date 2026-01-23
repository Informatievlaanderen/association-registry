namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using global::AutoFixture;
using AssociationRegistry.Test.Common.AutoFixture;

public class Faktory(IFixture fixture)
{
    public static Faktory New(IFixture? fixture = null) => new(fixture ?? new Fixture().CustomizeDomain());

    public VCodeServiceFactory VCodeService { get; } = new VCodeServiceFactory();
    public VerenigingsRepositoryFactory VerenigingsRepository { get; } = new VerenigingsRepositoryFactory();
    public NewAggregateSessionFactory NewAggregateSession { get; } = new NewAggregateSessionFactory();
    public ClockFactory Clock { get; } = new ClockFactory();
    public GeotagsServiceFactory GeotagsService { get; } = new GeotagsServiceFactory(fixture);

    public VerenigingenZonderGeotagsQueryFactory VerenigingenZonderGeotagsQuery { get; } =
        new VerenigingenZonderGeotagsQueryFactory(fixture);
    public MessageBusFactory MessageBus { get; } = new MessageBusFactory(fixture);
    public MartenOutboxFactory MartenOutbox { get; } = new MartenOutboxFactory(fixture);
    public postcodesFromGrarFetcherFactory postcodesFromGrarFetcher { get; } =
        new postcodesFromGrarFetcherFactory(fixture);
    public NutsLauFromGrarFetcherFactory nutsLauFromGrarFetcher { get; } = new NutsLauFromGrarFetcherFactory(fixture);

    public GrarClientFactory GrarClientFactory { get; } = new GrarClientFactory(fixture);
}
