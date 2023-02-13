namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using Marten;
using Marten.Events.Projections;
using Projections;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source)
    {
        source.Projections.Add(new VerenigingenPerInszProjection(), ProjectionLifecycle.Async);
    }
}
