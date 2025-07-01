namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using DuplicateVerenigingDetection;
using Vereniging;

public class NoDuplicateVerenigingDetectionService : IDuplicateVerenigingDetectionService
{
    public Task<IReadOnlyCollection<DuplicaatVereniging>> ExecuteAsync(VerenigingsNaam naam, Locatie[] locaties, bool includeScore = false, MinimumScore? minimumScoreOverride = null)
        => Task.FromResult<IReadOnlyCollection<DuplicaatVereniging>>(new List<DuplicaatVereniging>());

    public async Task<IReadOnlyCollection<DuplicaatVereniging>> ExecuteAsync(
        VerenigingsNaam naam,
        DuplicateVerenigingZoekQueryLocaties locaties,
        bool includeScore = false,
        MinimumScore? minimumScoreOverride = null)
        => [];
}
