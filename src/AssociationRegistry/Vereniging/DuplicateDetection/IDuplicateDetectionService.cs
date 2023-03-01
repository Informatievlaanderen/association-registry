namespace AssociationRegistry.Vereniging.DuplicateDetection;

using Locaties;
using VerenigingsNamen;

public interface IDuplicateDetectionService
{
    Task<IReadOnlyCollection<DuplicateCandidate>> GetDuplicates(VerenigingsNaam naam, LocatieLijst locaties);
}
