namespace AssociationRegistry.Admin.ProjectionHost.Projections.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
using Events;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;
using Schema;
using Schema.Relatieaanvragen;

/// <summary>
///     The relatieaanvragen, to query them per vCode (as koepel or as vereniging). A relatieaanvraag has its own
///     stream: the aanvraagId.
/// </summary>
public partial class RelatieaanvraagProjection : EventProjection
{
    public static readonly ShardName ShardName = new("beheer.postgres.relatieaanvraag");

    public RelatieaanvraagProjection()
    {
        Name = ShardName.Name;
        Options.BatchSize = 1;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<RelatieaanvraagDocument>();
    }

    public void Project(IEvent<RelatieaanvraagWerdIngediend> @event, IDocumentOperations ops) =>
        ops.Store(
            new RelatieaanvraagDocument
            {
                AanvraagId = @event.Data.AanvraagId,
                KoepelVCode = @event.Data.KoepelVCode,
                VerenigingVCode = @event.Data.VerenigingVCode,
                AangevraagdDoor = @event.Data.AangevraagdDoor,
                RelatieType = @event.Data.RelatieType,
                Status = RelatieaanvraagStatus.Aangevraagd.Value,
                AanvraagDatum = @event.Data.AanvraagDatum,
                VervalDatum = @event.Data.VervalDatum,
                Beschrijving = @event.Data.Beschrijving,
                Metadata = new Metadata(@event.Sequence, @event.Version),
            }
        );
}
