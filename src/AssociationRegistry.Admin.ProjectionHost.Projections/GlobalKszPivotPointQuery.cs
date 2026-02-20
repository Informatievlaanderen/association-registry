namespace AssociationRegistry.Admin.ProjectionHost.Queries;

using Events;
using Marten;
using NodaTime;

public interface IGlobalKszPivotPointQuery
{
    Instant? Execute();
}

public class GlobalKszPivotPointQuery : IGlobalKszPivotPointQuery
{
    private readonly Func<IQuerySession> _querySessionFactory;

    public GlobalKszPivotPointQuery(Func<IQuerySession> querySessionFactory)
    {
        _querySessionFactory = querySessionFactory;
    }

    public Instant? Execute()
    {
        using var session = _querySessionFactory();

        var kszEventTypes = new[]
        {
            typeof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens),
            typeof(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens),
            typeof(KszSyncHeeftVertegenwoordigerBevestigd),
        };

        var eersteKszEvent = session
            .Events.QueryAllRawEvents()
            .Where(e => kszEventTypes.Contains(e.EventType))
            .OrderBy(e => e.Timestamp)
            .FirstOrDefault();

        return eersteKszEvent != null ? Instant.FromDateTimeOffset(eersteKszEvent.Timestamp) : null;
    }
}
