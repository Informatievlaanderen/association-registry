namespace AssociationRegistry.Admin.Api.Infrastructure;

using Marten;
using Marten.Events.Projections;
using Projections;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source)
    {
        source.Projections.Add<VerenigingHistoriekProjection>(ProjectionLifecycle.Async);
    }
}
