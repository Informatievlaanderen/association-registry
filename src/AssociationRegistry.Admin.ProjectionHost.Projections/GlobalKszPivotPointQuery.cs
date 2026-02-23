namespace AssociationRegistry.Admin.ProjectionHost.Projections;

using Events;
using Marten;
using NodaTime;

public interface IGlobalKszPivotPointQuery
{
    Task<Instant?> ExecuteAsync();
}

public class GlobalKszPivotPointQuery : IGlobalKszPivotPointQuery
{
    private readonly Func<IQuerySession> _querySessionFactory;

    public GlobalKszPivotPointQuery(Func<IQuerySession> querySessionFactory)
    {
        _querySessionFactory = querySessionFactory;
    }

    public async Task<Instant?> ExecuteAsync()
    {
        using var session = _querySessionFactory();

        var kszEventTypes = new[]
        {
            "ksz_sync_heeft_vertegenwoordiger_aangeduid_als_overleden_zonder_persoonsgegevens",
            "ksz_sync_heeft_vertegenwoordiger_aangeduid_als_niet_gekend_zonder_persoonsgegevens",
            "ksz_sync_heeft_vertegenwoordiger_bevestigd",
        };

        var eersteKszEvent = (
            await session
                .Events.QueryAllRawEvents()
                .Where(e => kszEventTypes.Contains(e.EventTypeName))
                .OrderBy(e => e.Timestamp)
                .ToListAsync()
        ).FirstOrDefault();

        var allEvents = await session.Events.QueryAllRawEvents().ToListAsync();

        return eersteKszEvent != null ? Instant.FromDateTimeOffset(eersteKszEvent.Timestamp) : null;
    }
}
