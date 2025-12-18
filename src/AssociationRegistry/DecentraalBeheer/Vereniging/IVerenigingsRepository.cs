namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Events;
using EventStore;
using Framework;
using Marten;
using Persoonsgegevens;

public interface IVerenigingsRepository
{
    Task<StreamActionResult> Save(VerenigingsBase vereniging, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<StreamActionResult> Save(VerenigingsBase vereniging, IDocumentSession session, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<TVereniging> Load<TVereniging>(VCode vCode, CommandMetadata commandMetadata, bool allowVerwijderdeVereniging = false, bool allowDubbeleVereniging = false) where TVereniging : IHydrate<VerenigingState>, new();
    Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata);
    Task<bool> IsVerwijderd(VCode vCode);
    Task<bool> IsDubbel(VCode vCode);
    Task<bool> Exists(VCode vCode);
    Task<bool> Exists(KboNummer kboNummer);
    Task<StreamActionResult> SaveNew(VerenigingsBase vereniging, IDocumentSession session, CommandMetadata messageMetadata, CancellationToken cancellationToken);
    Task<IReadOnlyList<VCode>> FilterVzerOnly(IEnumerable<VCode> vCodes);
}
