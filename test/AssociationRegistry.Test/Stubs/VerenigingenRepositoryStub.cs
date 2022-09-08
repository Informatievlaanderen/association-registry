using System.Collections.Immutable;
using AssociationRegistry.Public.Api.ListVerenigingen;

namespace AssociationRegistry.Test.Stubs;

public class VerenigingenRepositoryStub : IVerenigingenRepository
{
    private readonly List<Vereniging> _verenigingen;

    public VerenigingenRepositoryStub(List<Vereniging> verenigingen)
    {
        _verenigingen = verenigingen;
    }

    public Task<ImmutableArray<Vereniging>> List() => Task.FromResult(_verenigingen.ToImmutableArray());
    public Task<int> TotalCount() => Task.FromResult(_verenigingen.Count);
}
