namespace AssociationRegistry.Admin.ProjectionHost.Projections.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Events;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;
using Schema.Erkenningen;

public partial class ErkenningProjection : EventProjection
{
    public static readonly ShardName ShardName = new("beheer.postgres.erkenning");

    public ErkenningProjection()
    {
        Name = ShardName.Name;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<ErkenningDocument>();
    }

    public void Project(IEvent<Archived> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;
        ops.DeleteWhere<ErkenningDocument>(x => x.VCode == vCode);
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

#pragma warning disable CS0618
    /// <summary>
    ///     Overgenomen erkenningen worden erkenningen van de authentieke vereniging, met hun nieuwe id, zodat ze ook
    ///     automatisch geactiveerd en verlopen worden. De erkenningen van de dubbele vereniging blijven bestaan.
    /// </summary>
    public void Project(
        IEvent<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> @event,
        IDocumentOperations ops
    )
    {
        var vCode = @event.StreamKey!;

        foreach (var erkenning in @event.Data.OvergenomenGegevens.Erkenningen.Select(x => x.NieuweWaarde))
            ops.Store(
                new ErkenningDocument
                {
                    Id = $"{vCode}-{erkenning.ErkenningId}",
                    VCode = vCode,
                    ErkenningId = erkenning.ErkenningId,
                    Startdatum = erkenning.Startdatum,
                    Einddatum = erkenning.Einddatum,
                    Status = erkenning.Status,
                }
            );
    }
#pragma warning restore CS0618

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

    public async Task Project(IEvent<ErkenningWerdVerlopen> @event, IDocumentOperations ops)
    {
        var vCode = @event.StreamKey!;
        var id = $"{vCode}-{@event.Data.ErkenningId}";

        var doc = await ops.LoadAsync<ErkenningDocument>(id);

        if (doc is null)
            return;

        doc.Status = ErkenningStatus.Verlopen.Value;
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
