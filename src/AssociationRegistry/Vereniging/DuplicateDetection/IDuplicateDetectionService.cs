namespace AssociationRegistry.Vereniging.DuplicateDetection;

using Locaties;
using VerenigingsNamen;

public interface IDuplicateDetectionService
{
    Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(VerenigingsNaam naam, LocatieLijst locaties);
}
