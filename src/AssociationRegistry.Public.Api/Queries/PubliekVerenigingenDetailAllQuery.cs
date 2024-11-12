namespace AssociationRegistry.Public.Api.Queries;

using Framework;
using Infrastructure.Extensions;
using Marten;
using Schema.Detail;
using Verenigingen.Detail;

public interface IPubliekVerenigingenDetailAllQuery : IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>>;

public class PubliekVerenigingenDetailAllQuery : IPubliekVerenigingenDetailAllQuery
{
    private readonly IDocumentSession _session;

    public PubliekVerenigingenDetailAllQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IAsyncEnumerable<PubliekVerenigingDetailDocument>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return _session.Query<PubliekVerenigingDetailDocument>()
                       .IncludeDeleted()
                       .ToAsyncEnumerable(cancellationToken);
    }
}
