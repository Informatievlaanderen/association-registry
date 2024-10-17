namespace AssociationRegistry.Acm.Api.Queries.VerenigingenPerInsz;

using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using Marten;

public class VerenigingenPerInszQuery : IVerenigingenPerInszQuery
{
    private readonly IDocumentSession _session;

    public VerenigingenPerInszQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<VerenigingenPerInszDocument> ExecuteAsync(VerenigingenPerInszFilter filter, CancellationToken cancellationToken)
    {
        return await _session.Query<VerenigingenPerInszDocument>()
                            .Where(x => x.Insz.Equals(filter.Insz, StringComparison.CurrentCultureIgnoreCase))
                            .SingleOrDefaultAsync(token: cancellationToken)
            ?? new VerenigingenPerInszDocument { Insz = filter.Insz };
    }
}
