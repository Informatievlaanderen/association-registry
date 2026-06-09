namespace AssociationRegistry.Admin.ProjectionHost.Projections.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Events;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;
using Schema.Erkenningen;

public class ErkenningProjection : EventProjection
{
    public static readonly ShardName ShardName = new("beheer.postgres.erkenning");

    public ErkenningProjection()
    {
        Name = ShardName.Name;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<ErkenningDocument>();
    }

    public void Project(IEvent<ErkenningWerdGeregistreerd> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;
        ops.Store(
            new ErkenningDocument
            {
                Id = $"{vCode}-{@event.Data.ErkenningId}",
                VCode = vCode,
                ErkenningId = @event.Data.ErkenningId,
                Startdatum = @event.Data.Startdatum,
                Einddatum = @event.Data.Einddatum,
                Status = @event.Data.Status,
            }
        );
    }

    public async Task Project(IEvent<ErkenningWerdGewijzigd> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;
        var id = $"{vCode}-{@event.Data.ErkenningId}";

        var doc = await ops.LoadAsync<ErkenningDocument>(id);

        if (doc is null)
            return;

        doc.Startdatum = @event.Data.Startdatum;
        doc.Einddatum = @event.Data.Einddatum;
        doc.Status = @event.Data.Status;
        ops.Store(doc);
    }

    public async Task Project(IEvent<ErkenningWerdGeactiveerd> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;
        var id = $"{vCode}-{@event.Data.ErkenningId}";

        var doc = await ops.LoadAsync<ErkenningDocument>(id);

        if (doc is null)
            return;

        doc.Status = ErkenningStatus.Actief.Value;
        ops.Store(doc);
    }

    public async Task Project(IEvent<ErkenningWerdGeschorst> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;
        var id = $"{vCode}-{@event.Data.ErkenningId}";

        var doc = await ops.LoadAsync<ErkenningDocument>(id);

        if (doc is null)
            return;

        doc.Status = ErkenningStatus.Geschorst.Value;
        ops.Store(doc);
    }

    public async Task Project(IEvent<ErkenningWerdVerwijderd> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;
        var id = $"{vCode}-{@event.Data.ErkenningId}";

        var doc = await ops.LoadAsync<ErkenningDocument>(id);

        if (doc is null)
            return;

        ops.Delete(doc);
    }

    public void Project(IEvent<VerenigingWerdVerwijderd> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;

        ops.DeleteWhere<ErkenningDocument>(x => x.VCode == vCode);
    }

    public async Task Project(IEvent<SchorsingVanErkenningWerdOpgeheven> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;
        var id = $"{vCode}-{@event.Data.ErkenningId}";

        var doc = await ops.LoadAsync<ErkenningDocument>(id);

        if (doc is null)
            return;

        doc.Status = @event.Data.Status;
        ops.Store(doc);
    }
}
