namespace AssociationRegistry.Repositories;

using Admin.Api.HostedServices.GeotagsInitialisation;
using Marten;


public class GeotagMigrationRepository
{
    private readonly IDocumentSession _session;

    public GeotagMigrationRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<bool> DidMigrationAlreadyRunToCompletion(CancellationToken cancellationToken)
        => await _session.Query<GeotagMigration>()
                         .AnyAsync(x => x.Id == new GeotagMigration().Id, cancellationToken);

    public void AddMigrationRecord()
    {
        _session.Insert(new GeotagMigration());
    }
}
