namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Marten.Linq.SoftDeletes;
using Schema;
using Schema.Constants;
using Schema.Detail;
using System;
using System.Linq;

// ReSharper disable once InconsistentNaming
public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode.Equals(vCode, StringComparison.CurrentCultureIgnoreCase));

    public static IQueryable<PubliekVerenigingDetailDocument> OnlyActief(this IQueryable<PubliekVerenigingDetailDocument> source)
        => source.Where(x => x.Status == VerenigingStatus.Actief);

    public static IQueryable<T> OnlyIngeschrevenInPubliekeDatastroom<T>(this IQueryable<T> source)
        where T : ICanBeUitgeschrevenUitPubliekeDatastroom
        => source.Where(x => !x.IsUitgeschrevenUitPubliekeDatastroom.HasValue ||
                             (x.IsUitgeschrevenUitPubliekeDatastroom.HasValue &&
                             !x.IsUitgeschrevenUitPubliekeDatastroom.Value));

    public static IQueryable<T> IncludeDeleted<T>(this IQueryable<T> source)
        where T : ICanBeUitgeschrevenUitPubliekeDatastroom
        => source.Where(x => x.MaybeDeleted());
}
