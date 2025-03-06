namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using DuplicateVerenigingDetection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vereniging;

public class NoDuplicateVerenigingDetectionService : IDuplicateVerenigingDetectionService
{
    public Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(VerenigingsNaam naam, Locatie[] locaties, bool includeScore = false, MinimumScore? minimumScoreOverride = null)
        => Task.FromResult<IReadOnlyCollection<DuplicaatVereniging>>(new List<DuplicaatVereniging>());
}
