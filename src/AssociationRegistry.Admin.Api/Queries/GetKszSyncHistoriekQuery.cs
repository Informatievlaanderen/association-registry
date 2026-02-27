namespace AssociationRegistry.Admin.Api.Queries;

using Framework;
using Marten;
using Schema.KboSync;

public interface IKszSyncHistoriekQuery : IQuery<BeheerKszSyncHistoriekGebeurtenisDocument[], KszSyncHistoriekFilter> { }

public class KszSyncHistoriekQuery : IKszSyncHistoriekQuery
{
    private readonly IDocumentSession _session;

    public KszSyncHistoriekQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<BeheerKszSyncHistoriekGebeurtenisDocument[]> ExecuteAsync(KszSyncHistoriekFilter filter, CancellationToken cancellationToken)
        => (await _session.Query<BeheerKszSyncHistoriekGebeurtenisDocument>().FilterOnVCode(filter.VCode)
                          .ToListAsync(cancellationToken)).ToArray();
}

public record KszSyncHistoriekFilter
{
    public string? VCode { get; }

    public KszSyncHistoriekFilter(string? vCode = null)
    {
        VCode = vCode;
    }
}
