namespace AssociationRegistry.Admin.ProjectionHost.Projections.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
using Events;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten.Events.Aggregation;
using Schema;
using Schema.Relatieaanvragen;

/// <summary>
///     The relatieaanvragen, to query them per vCode (as koepel or as vereniging). A relatieaanvraag has its own
///     stream: the aanvraagId.
/// </summary>
public partial class RelatieaanvraagProjection : SingleStreamProjection<RelatieaanvraagDocument, string>
{
    public static readonly ShardName ShardName = new("beheer.postgres.relatieaanvraag");

    public RelatieaanvraagProjection()
    {
        Name = ShardName.Name;
    }

    public RelatieaanvraagDocument Create(IEvent<RelatieaanvraagWerdIngediend> @event) =>
        new()
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
        };

    public void Apply(RelatieaanvraagDocument document, IEvent<RelatieaanvraagWerdBevestigd> @event)
    {
        document.Status = RelatieaanvraagStatus.Bevestigd.Value;
        document.Bevestiging = new RelatieaanvraagBevestiging { Datum = @event.Data.BevestigingsDatum };
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }
}
