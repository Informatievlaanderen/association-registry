namespace AssociationRegistry.Test.Projections.Framework;

using Alba;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Threading.Tasks;

public record ProjectionHostContext
{
    public ProjectionHostContext(IAlbaHost host)
    {
        Host = host;
        ElasticClient = Host.Services.GetRequiredService<IElasticClient>();
    }

    public IAlbaHost Host { get; }
    public IElasticClient ElasticClient { get; }

    public async Task RefreshDataAsync()
    {
        await Host.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await ElasticClient.Indices.RefreshAsync();
    }
}
