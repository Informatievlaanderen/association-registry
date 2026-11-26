namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Events;
using Exceptions;
using Framework;
using JasperFx;
using Vereniging;

/// <summary>The state of a relatieaanvraag, built from its own stream (the aanvraagId).</summary>
public record RelatieaanvraagState : IHasVersion
{
    /// <summary>The stream key: Marten needs an identity to aggregate the stream. The same as the aanvraagId.</summary>
    [Identity]
    public string Id { get; set; } = null!;

    /// <summary>Set by Marten: the version of the stream, for If-Match and optimistic concurrency.</summary>
    public long Version { get; set; }

    public RelatieaanvraagId AanvraagId { get; init; } = null!;
    public VCode KoepelVCode { get; init; } = null!;
    public VCode VerenigingVCode { get; init; } = null!;
    public VCode AangevraagdDoor { get; init; } = null!;
    public RelatieType RelatieType { get; init; } = null!;
    public RelatieaanvraagStatus Status { get; init; } = null!;
    public DateOnly VervalDatum { get; init; }

    /// <summary>The other partij than the aanvrager: only the ontvanger can confirm or refuse the relatieaanvraag.</summary>
    public VCode Ontvanger => AangevraagdDoor == KoepelVCode ? VerenigingVCode : KoepelVCode;

    /// <summary>
    ///     Only an open relatieaanvraag can be confirmed, refused or withdrawn: Aangevraagd, and the vervaldatum not over.
    ///     On the vervaldatum itself it is still open. After the vervaldatum it is vervallen, also when the job of the
    ///     scheduled host did not close it yet (story 13).
    /// </summary>
    public void ThrowIfNotOpen(DateOnly today)
    {
        Throw<RelatieaanvraagWerdAlBevestigd>.If(Status == RelatieaanvraagStatus.Bevestigd);
        Throw<RelatieaanvraagWerdAlGeweigerd>.If(Status == RelatieaanvraagStatus.Geweigerd);
        Throw<RelatieaanvraagWerdAlIngetrokken>.If(Status == RelatieaanvraagStatus.Ingetrokken);

        Throw<RelatieaanvraagIsVervallen>.If(
            Status == RelatieaanvraagStatus.Vervallen || Relatieaanvraag.IsTermijnVerstreken(VervalDatum, today)
        );
    }

    /// <summary>
    ///     Still Aangevraagd on the day after its vervaldatum: the job of the scheduled host lets it vervallen. Like
    ///     <c>Erkenning.KanVerlopenWordenOp</c>.
    /// </summary>
    public bool KanVervallenOp(DateOnly today) =>
        Status == RelatieaanvraagStatus.Aangevraagd && Relatieaanvraag.IsTermijnVerstreken(VervalDatum, today);

    /// <summary>
    ///     Only for a relatieaanvraag that <see cref="KanVervallenOp" /> today: check that first. It vervalt today, also
    ///     when the job runs later than the day after the vervaldatum. Nothing changes on the verenigingen. The caller
    ///     saves the event.
    /// </summary>
    public RelatieaanvraagWerdVervallen Verval(DateOnly today)
    {
        if (!KanVervallenOp(today))
            throw new RelatieaanvraagKanNietVervallen(AanvraagId, Status, VervalDatum);

        return new RelatieaanvraagWerdVervallen(AanvraagId.Value, today);
    }

    public RelatieaanvraagState Apply(RelatieaanvraagWerdIngediend @event) =>
        this with
        {
            Id = @event.AanvraagId,
            AanvraagId = RelatieaanvraagId.Hydrate(@event.AanvraagId),
            KoepelVCode = VCode.Hydrate(@event.KoepelVCode),
            VerenigingVCode = VCode.Hydrate(@event.VerenigingVCode),
            AangevraagdDoor = VCode.Hydrate(@event.AangevraagdDoor),
            RelatieType = RelatieType.Parse(@event.RelatieType),
            Status = RelatieaanvraagStatus.Aangevraagd,
            VervalDatum = @event.VervalDatum,
        };

    public RelatieaanvraagState Apply(RelatieaanvraagWerdBevestigd @event) =>
        this with
        {
            Status = RelatieaanvraagStatus.Bevestigd,
        };

    public RelatieaanvraagState Apply(RelatieaanvraagWerdGeweigerd @event) =>
        this with
        {
            Status = RelatieaanvraagStatus.Geweigerd,
        };

    public RelatieaanvraagState Apply(RelatieaanvraagWerdIngetrokken @event) =>
        this with
        {
            Status = RelatieaanvraagStatus.Ingetrokken,
        };

    public RelatieaanvraagState Apply(RelatieaanvraagWerdVervallen @event) =>
        this with
        {
            Status = RelatieaanvraagStatus.Vervallen,
        };
}
