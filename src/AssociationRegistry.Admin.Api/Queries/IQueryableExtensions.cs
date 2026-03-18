namespace AssociationRegistry.Admin.Api.Queries;

using Schema;
using Schema.Bewaartermijn;
using Schema.KboSync;

public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode.Equals(vCode, StringComparison.CurrentCultureIgnoreCase));

    public static IQueryable<BewaartermijnDocument> WithBewaartermijnId(this IQueryable<BewaartermijnDocument> source, string bewaartermijnId)
        => source.Where(x => x.BewaartermijnId.Equals(bewaartermijnId, StringComparison.CurrentCultureIgnoreCase));

    public static IQueryable<BeheerKszSyncHistoriekGebeurtenisDocument> FilterOnVCode(this IQueryable<BeheerKszSyncHistoriekGebeurtenisDocument> query, string? vCode)
    {
        if (vCode is null)
            return query;

        return query.Where(x => x.VCode.Equals(vCode));
    }
}
