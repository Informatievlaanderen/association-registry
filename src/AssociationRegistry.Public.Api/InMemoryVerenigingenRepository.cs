namespace AssociationRegistry.Public.Api;

using System.Collections.Immutable;
using System.Threading.Tasks;
using ListVerenigingen;

public class InMemoryVerenigingenRepository : IVerenigingenRepository
{
    private readonly Vereniging[] _verenigingen;

    public InMemoryVerenigingenRepository(params Vereniging[] verenigingen)
    {
        _verenigingen = verenigingen;
    }

    public Task<ImmutableArray<Vereniging>> List()
    {
        return Task.FromResult(_verenigingen.ToImmutableArray());
    }
}
