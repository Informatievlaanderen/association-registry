using System.Collections.Immutable;
using AssociationRegistry.Public.Api.DetailVerenigingen;
using AssociationRegistry.Public.Api.ListVerenigingen;

namespace AssociationRegistry.Test.Stubs;

public class VerenigingenRepositoryStub : IVerenigingenRepository
{
    private readonly List<VerenigingListItem> _verenigingen;
    private readonly List<VerenigingDetail> _verenigingenDetails;

    public VerenigingenRepositoryStub(List<VerenigingListItem> verenigingen)
    {
        _verenigingen = verenigingen;
        _verenigingenDetails = new List<VerenigingDetail>();
    }

    public VerenigingenRepositoryStub(List<VerenigingDetail> verenigingenDetails)
    {
        _verenigingenDetails = verenigingenDetails;
        _verenigingen = new List<VerenigingListItem>();
    }

    public Task<ImmutableArray<VerenigingListItem>> List() => Task.FromResult(_verenigingen.ToImmutableArray());
    public Task<VerenigingDetail?> Detail(string vCode) => Task.FromResult(_verenigingenDetails.FirstOrDefault());

    public Task<int> TotalCount() => Task.FromResult(_verenigingen.Count);
}
