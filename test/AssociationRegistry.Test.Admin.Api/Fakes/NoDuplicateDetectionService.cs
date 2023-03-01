namespace AssociationRegistry.Test.Admin.Api.Fakes;

using Locaties;
using Vereniging.DuplicateDetection;
using VerenigingsNamen;

public class NoDuplicateDetectionService : IDuplicateDetectionService
{
    public Task<IReadOnlyCollection<DuplicateCandidate>> GetDuplicates(VerenigingsNaam naam, LocatieLijst locaties)
        => Task.FromResult<IReadOnlyCollection<DuplicateCandidate>>(new List<DuplicateCandidate>());
}
