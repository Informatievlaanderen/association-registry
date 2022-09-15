using System.Collections.Immutable;
using System.Threading.Tasks;
using AssociationRegistry.Public.Api.DetailVerenigingen;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

public interface IVerenigingenRepository
{
    Task<ImmutableArray<VerenigingListItem>> List();
    Task<VerenigingDetail?> Detail(string vCode);
    Task<int> TotalCount();
}
