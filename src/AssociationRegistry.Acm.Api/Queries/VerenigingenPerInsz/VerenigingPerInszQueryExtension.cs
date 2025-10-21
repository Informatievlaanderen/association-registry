namespace AssociationRegistry.Acm.Api.Queries.VerenigingenPerInsz;

using Schema.VerenigingenPerInsz;

public static class VerenigingenPerInszQueryExtensions
{
    public static IQueryable<VerenigingenPerInszDocument> WithKboFilter(
        this IQueryable<VerenigingenPerInszDocument> query,
        bool includeKbo)
    {
        if (includeKbo) return query;

        return query.Where(d => d.Verenigingen.Any(v => string.IsNullOrEmpty(v.KboNummer)));
    }
}
