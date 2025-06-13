namespace AssociationRegistry.DuplicateVerenigingDetection;

using Vereniging;

public interface IDuplicateVerenigingDetectionService
{
    Task<IReadOnlyCollection<DuplicaatVereniging>> ExecuteAsync(
        VerenigingsNaam naam,
        Locatie[] locaties,
        bool includeScore = false,
        MinimumScore? minimumScoreOverride = null);
}
