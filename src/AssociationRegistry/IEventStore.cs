namespace AssociationRegistry;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;

public interface IEventStore
{
    Task<StreamActionResult> Save(
        string aggregateId,
        long? aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events
    );

    /// <summary>
    ///     Bewaart de events van meerdere aggregates in één transactie: alle streams worden bewaard, of geen enkele.
    ///     Conflicten worden niet opgelost: werd een stream intussen gewijzigd, dan wordt niets bewaard.
    ///     Geeft per aggregate het resultaat terug, in dezelfde volgorde.
    /// </summary>
    Task<IReadOnlyList<StreamActionResult>> SaveTransactional(
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params AggregateEvents[] aggregates
    );

    Task<T> Load<T>(string id, long? expectedVersion)
        where T : class, IHasVersion, new();

    Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion)
        where T : class, IHasVersion, new();

    Task<StreamActionResult> SaveNew(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events
    );

    Task<VCode?> GetVCodeForKbo(string kboNummer);

    Task<StreamActionResult> SaveNewKbo(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events
    );
}
