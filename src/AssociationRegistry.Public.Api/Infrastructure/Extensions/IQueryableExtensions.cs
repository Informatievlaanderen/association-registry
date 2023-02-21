namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Schema;

// ReSharper disable once InconsistentNaming
public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode.Equals(vCode, StringComparison.CurrentCultureIgnoreCase));
}
