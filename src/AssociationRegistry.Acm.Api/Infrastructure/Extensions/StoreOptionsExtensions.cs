namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using JasperFx.Events.Projections;
using Marten;
using Projections;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source)
    {
        source.Projections.Add(new VerenigingenPerInszProjection(), ProjectionLifecycle.Async);
    }
}
