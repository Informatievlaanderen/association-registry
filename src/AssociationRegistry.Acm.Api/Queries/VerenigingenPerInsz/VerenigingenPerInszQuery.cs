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

    public async Task<VerenigingenPerInszDocument> ExecuteAsync(
        VerenigingenPerInszFilter filter,
        CancellationToken cancellationToken)
    {
        var verenigingenPerInszDocument = await _session.Query<VerenigingenPerInszDocument>()
                                                        .Where(x => x.Insz.Equals(filter.Insz,
                                                                   StringComparison.CurrentCultureIgnoreCase))
                                                        .SingleOrDefaultAsync(token: cancellationToken)
                                       ?? new VerenigingenPerInszDocument { Insz = filter.Insz };

        return verenigingenPerInszDocument
            with
            {
                Verenigingen = FilterKboVerenigingen(verenigingenPerInszDocument, filter.IncludeKboVerenigingen),
            };
    }

    private static List<Vereniging> FilterKboVerenigingen(VerenigingenPerInszDocument verenigingenPerInszDocument, bool includeKboVerenigingen)
    {
        return verenigingenPerInszDocument
              .Verenigingen
              .Where(v => includeKboVerenigingen || string.IsNullOrEmpty(v.KboNummer))
              .ToList();
    }
}
