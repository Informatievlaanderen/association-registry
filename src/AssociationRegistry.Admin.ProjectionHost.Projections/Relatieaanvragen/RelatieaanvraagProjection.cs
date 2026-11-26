namespace AssociationRegistry.Admin.ProjectionHost.Projections.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
using Events;
using Formats;
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

    public void Apply(RelatieaanvraagDocument document, IEvent<RelatieaanvraagWerdBevestigd> @event) =>
        Afgehandeld(document, RelatieaanvraagStatus.Bevestigd, @event.Data.BevestigingsDatum, @event);

    public void Apply(RelatieaanvraagDocument document, IEvent<RelatieaanvraagWerdGeweigerd> @event) =>
        Afgehandeld(document, RelatieaanvraagStatus.Geweigerd, @event.Data.WeigeringsDatum, @event);

    public void Apply(RelatieaanvraagDocument document, IEvent<RelatieaanvraagWerdIngetrokken> @event) =>
        Afgehandeld(document, RelatieaanvraagStatus.Ingetrokken, @event.Data.IntrekkingsDatum, @event);

    public void Apply(RelatieaanvraagDocument document, IEvent<RelatieaanvraagWerdVervallen> @event) =>
        Afgehandeld(document, RelatieaanvraagStatus.Vervallen, @event.Data.VervalDatum, @event);

    private static void Afgehandeld(
        RelatieaanvraagDocument document,
        RelatieaanvraagStatus status,
        DateOnly afhandelDatum,
        JasperFx.Events.IEvent @event
    )
    {
        document.Status = status.Value;
        document.AfhandelDatum = afhandelDatum.ToString(WellknownFormats.DateOnly);
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }
}
