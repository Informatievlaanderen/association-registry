namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using System.Linq;
using System.Threading.Tasks;
using AssociationRegistry.Admin.Api.Projections;
using Marten;

// ReSharper disable once InconsistentNaming
public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode == vCode);
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
