namespace AssociationRegistry.Admin.Api.Infrastructure;

using System;
using Marten;
using Marten.Events.Projections;
using Microsoft.Extensions.DependencyInjection;
using ProjectionHost.Projections.Detail;
using ProjectionHost.Projections.Historiek;
using Projections.Search;
using Wolverine;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source, IServiceProvider serviceProvider)
    {
        source.Projections.Add<BeheerVerenigingHistoriekProjection>(ProjectionLifecycle.Async);
        source.Projections.Add<BeheerVerenigingDetailProjection>(ProjectionLifecycle.Async);
        source.Projections.Add(
            new MartenSubscription(
                new MartenEventsConsumer(
                    serviceProvider.GetRequiredService<IMessageBus>()
                )
            ),
            ProjectionLifecycle.Async,
            "BeheerVerenigingZoekenDocument");
    }
}
