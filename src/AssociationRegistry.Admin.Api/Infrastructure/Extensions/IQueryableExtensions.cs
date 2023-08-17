namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Marten.Events.Daemon;
using Microsoft.AspNetCore.Http;
using Schema;
using Schema.Detail;
using Schema.Historiek;

// ReSharper disable once InconsistentNaming
public static class IQueryableExtensions
{
    public static IQueryable<T> WithVCode<T>(this IQueryable<T> source, string vCode) where T : IVCode
        => source.Where(x => x.VCode.Equals(vCode, StringComparison.CurrentCultureIgnoreCase));
}

// ReSharper disable once InconsistentNaming
public static class IDocumentExtensions
{
    public static async Task<bool> HasReachedSequence<T>(this IDocumentStore documentStore, long? expectedSequence) where T : IMetadata
    {
        if (expectedSequence == null)
            return true;

        var shardName = GetShardName<T>();
        var sequenceReached = await documentStore.Advanced.ProjectionProgressFor(shardName);

        return expectedSequence <= sequenceReached;
    }

    private static ShardName GetShardName<T>() where T : IMetadata
    {
        if (typeof(T) == typeof(BeheerVerenigingDetailDocument))
        {
            return new ShardName("AssociationRegistry.Admin.ProjectionHost.Projections.Detail.BeheerVerenigingDetailProjection");
        }

        if (typeof(T) == typeof(BeheerVerenigingHistoriekDocument))
        {
            return new ShardName("AssociationRegistry.Admin.ProjectionHost.Projections.Historiek.BeheerVerenigingHistoriekProjection");
        }

        throw new NotImplementedException("ExpectedSequence is voor deze projectie niet ondersteund.");
    }
}
