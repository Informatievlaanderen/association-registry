namespace AssociationRegistry.Admin.Api.Queries;

using Schema;
using Schema.Persoonsgegevens;

public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode.Equals(vCode, StringComparison.CurrentCultureIgnoreCase));
    public static IQueryable<VertegenwoordigerPersoonsgegevensDocument> ForRefId(this IQueryable<VertegenwoordigerPersoonsgegevensDocument> source, Guid refId)
        => source.Where(x => x.RefId == refId);
}
