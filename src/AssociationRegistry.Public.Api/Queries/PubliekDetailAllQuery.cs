namespace AssociationRegistry.Public.Api.Queries;

using Infrastructure.Extensions;
using Marten;
using Schema.Detail;
using Verenigingen.Detail;

public class PubliekDetailAllQuery : IPubliekVerenigingenDetailAllQuery
{
    private readonly IDocumentSession _session;

    public PubliekDetailAllQuery(IDocumentSession session)
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
