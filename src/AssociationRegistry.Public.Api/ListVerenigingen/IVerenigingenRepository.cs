namespace AssociationRegistry.Public.Api.ListVerenigingen;

using System.Collections.Immutable;
using System.Threading.Tasks;

public interface IVerenigingenRepository
{
    Task<ImmutableArray<VerenigingListItem>> List();
    Task<int> TotalCount();
}
