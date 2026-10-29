namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using DecentraalBeheer.Vereniging;
using Framework;
using Marten;

public interface IAggregateSession
{
    Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default
    );

    Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Bewaart meerdere verenigingen in één transactie: alle verenigingen worden bewaard, of geen enkele.
    ///     Geeft per vereniging het resultaat terug, in dezelfde volgorde.
    /// </summary>
    Task<IReadOnlyList<StreamActionResult>> SaveTransactional(
        IReadOnlyCollection<VerenigingsBase> verenigingen,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default
    );

    Task<TVereniging> Load<TVereniging>(
        VCode vCode,
        CommandMetadata metadata,
        bool allowVerwijderdeVereniging = false,
        bool allowDubbeleVereniging = false
    )
        where TVereniging : IHydrate<VerenigingState>, new();

    Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata);
}
