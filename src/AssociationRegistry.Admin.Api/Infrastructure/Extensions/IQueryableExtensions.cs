namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Projections;
using Schema;

// ReSharper disable once InconsistentNaming
public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode.Equals(vCode, StringComparison.CurrentCultureIgnoreCase));
}

// ReSharper disable once InconsistentNaming
public static class IDocumentSessionExtensions
{
    public static async Task<bool> HasReachedSequence<T>(this IDocumentSession source, long? expectedSequence) where T : IMetadata
        => expectedSequence == null ||
           await source
               .Query<T>()
               .MaxAsync(document => document.Metadata.Sequence) >= expectedSequence;
}
