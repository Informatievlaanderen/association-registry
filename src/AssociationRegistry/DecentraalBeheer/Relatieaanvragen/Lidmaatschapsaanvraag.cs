namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Events;
using Exceptions;
using Framework;
using Vereniging;
using Vereniging.Subtypes.Subvereniging;

/// <summary>A relatieaanvraag to become lid of the koepel.</summary>
public sealed class Lidmaatschapsaanvraag : Relatieaanvraag
{
    private Lidmaatschapsaanvraag(RelatieaanvraagWerdIngediend ingediend)
        : base(ingediend) { }

    public static async Task<Lidmaatschapsaanvraag> Create(
        Partijen partijen,
        BeoogdeKoepel koepel,
        VerenigingState vereniging,
        string beschrijving,
        OpenRelatieaanvragen openRelatieaanvragen,
        IRelatieaanvraagIdService aanvraagIdService,
        int vervaltermijnInDagen,
        DateOnly vandaag
    )
    {
        CheckRules(koepel, vereniging, openRelatieaanvragen, vandaag);

        return new Lidmaatschapsaanvraag(
            await Ingediend(
                partijen,
                RelatieType.Lidmaatschap,
                beschrijving,
                aanvraagIdService,
                vervaltermijnInDagen,
                vandaag
            )
        );
    }

    /// <summary>
    ///     The rules are checked again, on what may have changed since the relatieaanvraag was ingediend. The
    ///     lidmaatschap starts today, without einddatum.
    /// </summary>
    /// <param name="otherOpenRelatieaanvragen">Without the relatieaanvraag itself: it is still open.</param>
    public static void Bevestig(
        BeoogdeKoepel koepel,
        VerenigingOfAnyKind vereniging,
        OpenRelatieaanvragen otherOpenRelatieaanvragen,
        Relatiegegevens relatiegegevens,
        DateOnly today
    )
    {
        CheckRules(koepel, vereniging.CurrentState, otherOpenRelatieaanvragen, today);

        vereniging.WordLidVanKoepel(
            koepel.VCode,
            koepel.Naam,
            today,
            LidmaatschapIdentificatie.Create(relatiegegevens.Identificatie),
            LidmaatschapBeschrijving.Create(relatiegegevens.Beschrijving)
        );
    }

    /// <summary>
    ///     The rules of a lidmaatschapsaanvraag, in the order of the story.
    ///     <list type="bullet">
    ///         <item>A new lidmaatschap starts on confirmation without einddatum: every lidmaatschap with the koepel that has not ended blocks it.</item>
    ///         <item>K6: a subvereniging cannot become lid, also not through an open subverenigingsaanvraag.</item>
    ///     </list>
    /// </summary>
    private static void CheckRules(
        BeoogdeKoepel koepel,
        VerenigingState vereniging,
        OpenRelatieaanvragen openRelatieaanvragen,
        DateOnly today
    )
    {
        ControleerGeenActieveRelatie(vereniging, koepel, today);

        Throw<ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel>.If(
            LidmaatschappenBij(vereniging, koepel).Any(lidmaatschap => IsNietBeeindigd(lidmaatschap, today))
        );

        ControleerGeenOpenRelatieaanvraagVoorKoepelEnVereniging(RelatieType.Lidmaatschap, openRelatieaanvragen);

        Throw<SubverenigingKanGeenLidWorden>.If(vereniging.Verenigingssubtype is SubverenigingSubtype);

        if (openRelatieaanvragen.Subverenigingsaanvraag is { } openSubverenigingsaanvraag)
            throw new VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden(openSubverenigingsaanvraag.AanvraagId);
    }
}
