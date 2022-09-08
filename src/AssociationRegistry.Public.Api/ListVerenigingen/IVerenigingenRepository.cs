using System.Collections.Immutable;
using System.Threading.Tasks;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

public interface IVerenigingenRepository
{
    Task<ImmutableArray<Vereniging>> List();
    Task<int> TotalCount();
}
