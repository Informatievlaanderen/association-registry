namespace AssociationRegistry.Admin.Api.Infrastructure;

using Marten;
using Marten.Events.Projections;
using ProjectionHost.Projections.Detail;
using ProjectionHost.Projections.Historiek;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source)
    {
        source.Projections.Add<BeheerVerenigingHistoriekProjection>(ProjectionLifecycle.Async);
        source.Projections.Add<BeheerVerenigingDetailProjection>(ProjectionLifecycle.Async);
    }
}
