namespace AssociationRegistry.Admin.Api.Queries;

using Schema;
using Schema.Bewaartermijn;

public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode.Equals(vCode, StringComparison.CurrentCultureIgnoreCase));

    public static IQueryable<BewaartermijnDocument> WithBewaartermijnId(this IQueryable<BewaartermijnDocument> source, string bewaartermijnId)
        => source.Where(x => x.BewaartermijnId.Equals(bewaartermijnId, StringComparison.CurrentCultureIgnoreCase));
}
