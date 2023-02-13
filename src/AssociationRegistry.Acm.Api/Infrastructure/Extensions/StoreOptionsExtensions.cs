namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using Marten;
using Projections;

public static class StoreOptionsExtensions
{
    public static void AddPostgresProjections(this StoreOptions source)
    {
        source.Projections.Add<VerenigingenPerInszProjection>();
    }
}
