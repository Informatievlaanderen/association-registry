namespace AssociationRegistry.Admin.Api.Infrastructure;

using Marten;
using Marten.Events.Projections;
using Projections.Detail;
using Projections.Historiek;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source)
    {
        source.Projections.Add<VerenigingHistoriekProjection>(ProjectionLifecycle.Async);
        source.Projections.Add<VerenigingDetailProjection>(ProjectionLifecycle.Async);
    }
}
