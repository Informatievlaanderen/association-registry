namespace AssociationRegistry.Test.Projections.Queries;

using Admin.ProjectionHost.Projections;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using Marten;
using NodaTime;
using IEvent = JasperFx.Events.IEvent;

public class GlobalKszPivotPointQueryClassFixture : IAsyncLifetime
{
    public DocumentStore Store { get; private set; }
    public Fixture Fixture { get; }

    public GlobalKszPivotPointQueryClassFixture()
    {
        Fixture = new AutoFixture.Fixture().CustomizeDomain();
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.CreateAsync(nameof(GlobalKszPivotPointQueryTests).ToLower());
    }
}

public class GlobalKszPivotPointQueryTests : IClassFixture<GlobalKszPivotPointQueryClassFixture>, IAsyncLifetime
{
    private readonly DocumentStore _store;
    private readonly Fixture _fixture;

    public GlobalKszPivotPointQueryTests(GlobalKszPivotPointQueryClassFixture classFixture)
    {
        _store = classFixture.Store;
        _fixture = classFixture.Fixture;
    }

    public async ValueTask InitializeAsync() => await _store.Advanced.ResetAllData();

    [Fact]
    public async ValueTask Given_NoEvents_FindsNoEvents()
    {
        await AddIrrelevantEvent(_store);

        var query = new GlobalKszPivotPointQuery(() => _store.QuerySession());
        var pivotPoint = await query.ExecuteAsync();

        pivotPoint.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(KszEvents))]
    public async ValueTask Given_KszEvent_FindsEvents(Type eventType)
    {
        var kszEvent = await AddKszEvent(_store, eventType);

        var query = new GlobalKszPivotPointQuery(() => _store.QuerySession());
        var pivotPoint = await query.ExecuteAsync();

        var expected = Instant.FromUnixTimeSeconds(kszEvent.Timestamp.ToUnixTimeSeconds());
        var actual = Instant.FromUnixTimeSeconds(pivotPoint.Value.ToUnixTimeSeconds());

        actual.Should().Be(expected);
    }

    public static IEnumerable<object[]> KszEvents =>
        new List<object[]>
        {
            new object[] { typeof(KszSyncHeeftVertegenwoordigerBevestigd) },
            new object[] { typeof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens) },
            new object[] { typeof(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens) },
        };

    private async Task<IEvent> AddKszEvent(IDocumentStore store, Type eventType)
    {
        var vCode = _fixture.Create<VCode>();

        await using var session = store.LightweightSession();

        var context = new SpecimenContext(_fixture);
        var evt = context.Resolve(eventType);

        var streamAction = session.Events.Append(vCode, evt);

        await session.SaveChangesAsync();

        return streamAction.Events.First();
    }

    private async Task AddIrrelevantEvent(IDocumentStore store)
    {
        var vCode = _fixture.Create<VCode>();

        await using var session = store.LightweightSession();
        session.Events.Append(vCode, _fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>());

        await session.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync() { }
}
