namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using System;
using System.Linq;
using Schema;
using Schema.Constants;
using Schema.Detail;

// ReSharper disable once InconsistentNaming
public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode.Equals(vCode, StringComparison.CurrentCultureIgnoreCase));

    public static IQueryable<PubliekVerenigingDetailDocument> OnlyActief(this IQueryable<PubliekVerenigingDetailDocument> source)
        => source.Where(x => x.Status == VerenigingStatus.Actief);

     public static IQueryable<T> OnlyIngeschrevenInPubliekeDatastroom<T>(this IQueryable<T> source) where T : ICanBeUitgeschrevenUitPubliekeDatastroom
        => source.Where(x=>!x.IsUitgeschrevenUitPubliekeDatastroom);
}
