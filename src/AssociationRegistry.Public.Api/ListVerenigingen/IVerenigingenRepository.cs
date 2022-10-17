namespace AssociationRegistry.Public.Api.ListVerenigingen;

using System.Collections.Immutable;
using System.Threading.Tasks;
using DetailVerenigingen;

public interface IVerenigingenRepository
{
    Task<ImmutableArray<VerenigingListItem>> List();
    Task<VerenigingDetail?> Detail(string vCode);
    Task<int> TotalCount();
}
