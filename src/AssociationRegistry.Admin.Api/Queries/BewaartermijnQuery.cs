namespace AssociationRegistry.Admin.Api.Queries;

using Framework;
using Marten;
using Marten.Linq.SoftDeletes;
using Schema.Bewaartermijn;

public interface IBewaartermijnQuery : IQuery<BewaartermijnDocument?, BewaartermijnFilter>;

public class BewaartermijnQuery : IBewaartermijnQuery
{
    private readonly IDocumentSession _session;

    public BewaartermijnQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<BewaartermijnDocument?> ExecuteAsync(BewaartermijnFilter filter, CancellationToken cancellationToken)
        => await _session.Query<BewaartermijnDocument>()
                         .WithBewaartermijnId(filter.BewaartermijnId)
                         .SingleOrDefaultAsync(token: cancellationToken);
}

public record BewaartermijnFilter
{
    public string BewaartermijnId { get; }

    public BewaartermijnFilter(string bewaartermijnId)
    {
        BewaartermijnId = bewaartermijnId ?? throw new ArgumentNullException(nameof(bewaartermijnId));
    }
}
