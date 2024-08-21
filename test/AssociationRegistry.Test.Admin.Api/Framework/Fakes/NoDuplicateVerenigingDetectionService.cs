namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using DuplicateVerenigingDetection;
using Vereniging;

public class NoDuplicateVerenigingDetectionService : IDuplicateVerenigingDetectionService
{
    public Task<IReadOnlyCollection<DuplicaatVereniging>> GetDuplicates(VerenigingsNaam naam, Locatie[] locaties)
        => Task.FromResult<IReadOnlyCollection<DuplicaatVereniging>>(new List<DuplicaatVereniging>());
}
