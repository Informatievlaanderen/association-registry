namespace AssociationRegistry.Public.Api.Infrastructure;

using Projections;
using Marten;
using Marten.Events.Projections;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source)
    {
        source.Projections.Add<VerenigingDetailProjection>(ProjectionLifecycle.Async);
    }
}
