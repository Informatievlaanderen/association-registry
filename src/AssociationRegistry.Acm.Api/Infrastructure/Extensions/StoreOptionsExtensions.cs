namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using Marten;
using Marten.Events.Projections;
using Metrics;
using Microsoft.Extensions.DependencyInjection;
using Projections;
using System;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source, IServiceProvider serviceProvider)
    {
        source.Projections.Add(new VerenigingenPerInszProjection(), ProjectionLifecycle.Async);
        source.Projections.AsyncListeners.Add(new HighWatermarkListener(serviceProvider.GetRequiredService<Instrumentation>()));
    }
}
