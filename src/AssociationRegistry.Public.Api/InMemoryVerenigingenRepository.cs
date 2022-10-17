namespace AssociationRegistry.Public.Api;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DetailVerenigingen;
using ListVerenigingen;

public class InMemoryVerenigingenRepository : IVerenigingenRepository
{
    private readonly VerenigingListItem[] _verenigingenListItems;
    private readonly VerenigingDetail[] _verenigingenDetails;

    public InMemoryVerenigingenRepository(
        VerenigingListItem[] verenigingenListItems,
        VerenigingDetail[] verenigingenDetails)
    {
        _verenigingenListItems = verenigingenListItems;
        _verenigingenDetails = verenigingenDetails;
    }

    public Task<ImmutableArray<VerenigingListItem>> List() =>
        Task.FromResult(_verenigingenListItems.ToImmutableArray());

    public Task<VerenigingDetail?> Detail(string vCode) =>
        Task.FromResult(_verenigingenDetails.FirstOrDefault(v =>
            string.Equals(v.Id, vCode, StringComparison.OrdinalIgnoreCase)));

    public Task<int> TotalCount() => Task.FromResult(_verenigingenListItems.Length);
}
