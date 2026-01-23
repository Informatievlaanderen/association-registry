namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Events;
using EventStore;
using Framework;
using Marten;
using Persoonsgegevens;

public interface IVerenigingsRepository
{
    public Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );

    public Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );

    public Task<TVereniging> Load<TVereniging>(
        VCode vCode,
        CommandMetadata commandMetadata,
        bool allowVerwijderdeVereniging = false,
        bool allowDubbeleVereniging = false
    )
        where TVereniging : IHydrate<VerenigingState>, new();

    public Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata);

    public Task<StreamActionResult> SaveNew(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata messageMetadata,
        CancellationToken cancellationToken
    );
}
