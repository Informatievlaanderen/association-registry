namespace AssociationRegistry.Admin.Api.Queries;

using Framework;
using Marten;
using Marten.Linq.SoftDeletes;
using MartenDb;
using Schema.Detail;

public interface IBeheerVerenigingDetailQuery : IQuery<BeheerVerenigingDetailDocument?, BeheerVerenigingDetailFilter>;

public class BeheerVerenigingDetailQuery : IBeheerVerenigingDetailQuery
{
    private readonly IDocumentSession _session;

    public BeheerVerenigingDetailQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<BeheerVerenigingDetailDocument?> ExecuteAsync(BeheerVerenigingDetailFilter filter, CancellationToken cancellationToken)
        => await _session.Query<BeheerVerenigingDetailDocument>()
                         .Where(x => x.MaybeDeleted())
                         .WithVCode(filter.VCode)
                         .SingleOrDefaultAsync(token: cancellationToken);
}

public record BeheerVerenigingDetailFilter
{
    public string VCode { get; }

    public BeheerVerenigingDetailFilter(string vCode)
    {
        VCode = vCode ?? throw new ArgumentNullException(nameof(vCode));
    }
}
