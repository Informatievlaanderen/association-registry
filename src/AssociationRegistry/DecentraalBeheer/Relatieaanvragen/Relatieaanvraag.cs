namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Events;
using Exceptions;
using Framework;
using Vereniging;

/// <summary>
///     A request to register a relation between a koepel and a vereniging: a <see cref="Lidmaatschapsaanvraag" /> or a
///     <see cref="Subverenigingsaanvraag" />, each with its own rules. It lives in its own stream and changes nothing on
///     the verenigingen: the relation itself is only made when it is confirmed.
/// </summary>
public abstract class Relatieaanvraag
{
    /// <summary>
    ///     A relatieaanvraag is vervallen the day after its vervaldatum: on the vervaldatum itself it is still open. Uses
    ///     the vervaldatum that was saved when it was ingediend, so a change of the termijn does not move it.
    /// </summary>
    public static bool IsTermijnVerstreken(DateOnly vervalDatum, DateOnly vandaag) => vandaag > vervalDatum;

    protected Relatieaanvraag(RelatieaanvraagWerdIngediend ingediend)
    {
        AanvraagId = RelatieaanvraagId.Hydrate(ingediend.AanvraagId);
        KoepelVCode = VCode.Hydrate(ingediend.KoepelVCode);
        VerenigingVCode = VCode.Hydrate(ingediend.VerenigingVCode);
        RelatieType = RelatieType.Parse(ingediend.RelatieType);
        UncommittedEvents = [ingediend];
    }

    public RelatieaanvraagId AanvraagId { get; }
    public VCode KoepelVCode { get; }
    public VCode VerenigingVCode { get; }
    public RelatieType RelatieType { get; }
    public IReadOnlyCollection<IEvent> UncommittedEvents { get; }

    /// <summary>
    ///     The partijen were checked by their value object, before the verenigingen were loaded. Then the koepel, and the
    ///     rules of the type of relatieaanvraag.
    /// </summary>
    public static async Task<Relatieaanvraag> DienIn(
        Partijen partijen,
        VerenigingsBase koepel,
        VerenigingsBase vereniging,
        RelatieType relatieType,
        string beschrijving,
        OpenRelatieaanvragen openRelatieaanvragen,
        IRelatieaanvraagIdService aanvraagIdService,
        int vervaltermijnInDagen,
        IClock clock
    )
    {
        var beoogdeKoepel = BeoogdeKoepel.Create(koepel.CurrentState);

        return relatieType.IsSubvereniging
            ? await Subverenigingsaanvraag.Create(
                partijen,
                beoogdeKoepel,
                vereniging.CurrentState,
                beschrijving,
                openRelatieaanvragen,
                aanvraagIdService,
                vervaltermijnInDagen,
                clock.Today
            )
            : await Lidmaatschapsaanvraag.Create(
                partijen,
                beoogdeKoepel,
                vereniging.CurrentState,
                beschrijving,
                openRelatieaanvragen,
                aanvraagIdService,
                vervaltermijnInDagen,
                clock.Today
            );
    }

    /// <summary>A lidmaatschap with the koepel that runs today, or a subvereniging of the koepel, blocks every type.</summary>
    protected static void ControleerGeenActieveRelatie(
        VerenigingState vereniging,
        BeoogdeKoepel koepel,
        DateOnly vandaag
    ) =>
        Throw<ErBestaatAlEenActieveFeitelijkeRelatie>.If(
            LidmaatschappenBij(vereniging, koepel)
                .Any(lidmaatschap => lidmaatschap.Geldigheidsperiode.OverlapsWith(vandaag))
                || vereniging.Verenigingssubtype?.IsSubverenigingVan(koepel.VCode) == true
        );

    /// <summary>One open relatieaanvraag per koepel and vereniging, whoever asked it.</summary>
    protected static void ControleerGeenOpenRelatieaanvraagVoorKoepelEnVereniging(
        RelatieType relatieType,
        OpenRelatieaanvragen openRelatieaanvragen
    )
    {
        if (openRelatieaanvragen.VoorKoepelEnVereniging is not { } openRelatieaanvraag)
            return;

        if (openRelatieaanvraag.RelatieType == relatieType)
            throw new RelatieaanvraagVanHetzelfdeTypeStaatAlOpen(openRelatieaanvraag.AanvraagId);

        throw new RelatieaanvraagVanEenAnderTypeStaatAlOpen();
    }

    protected static IEnumerable<Lidmaatschap> LidmaatschappenBij(VerenigingState vereniging, BeoogdeKoepel koepel) =>
        vereniging.Lidmaatschappen.Where(lidmaatschap => lidmaatschap.AndereVereniging == koepel.VCode);

    // Checked on the einddatum: a lidmaatschap that only starts in the future has not ended either.
    protected static bool IsNietBeeindigd(Lidmaatschap lidmaatschap, DateOnly vandaag) =>
        lidmaatschap.Geldigheidsperiode.Tot.DateOnly is not { } einddatum || einddatum >= vandaag;

    /// <summary>The aanvraagId is only asked after the rules: a refused relatieaanvraag does not use a number.</summary>
    protected static async Task<RelatieaanvraagWerdIngediend> Ingediend(
        Partijen partijen,
        RelatieType relatieType,
        string beschrijving,
        IRelatieaanvraagIdService aanvraagIdService,
        int vervaltermijnInDagen,
        DateOnly vandaag
    )
    {
        var aanvraagId = await aanvraagIdService.GetNext();

        return new RelatieaanvraagWerdIngediend(
            aanvraagId.Value,
            partijen.KoepelVCode.Value,
            partijen.VerenigingVCode.Value,
            partijen.AangevraagdDoor.Value,
            relatieType.Value,
            beschrijving,
            vandaag,
            vandaag.AddDays(vervaltermijnInDagen)
        );
    }
}
