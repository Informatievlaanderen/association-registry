namespace AssociationRegistry.Vereniging;

using EventStore;
using Framework;
using Marten;

public interface IVerenigingsRepository
{
    Task<StreamActionResult> Save(VerenigingsBase vereniging, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<StreamActionResult> Save(VerenigingsBase vereniging, IDocumentSession session, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion = null, bool allowVerwijderdeVereniging = false, bool allowDubbeleVereniging = false) where TVereniging : IHydrate<VerenigingState>, new();
    Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, long? expectedVersion = null);
    Task<bool> IsVerwijderd(VCode vCode);
    Task<bool> IsDubbel(VCode vCode);
}
