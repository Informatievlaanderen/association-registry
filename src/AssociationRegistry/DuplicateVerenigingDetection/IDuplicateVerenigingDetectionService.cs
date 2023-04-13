namespace AssociationRegistry.DuplicateVerenigingDetection;

using Vereniging;

public interface IDuplicateVerenigingDetectionService
{
    Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(VerenigingsNaam naam, Locatie[] locaties);
}
