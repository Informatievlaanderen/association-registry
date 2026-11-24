namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Events;
using Exceptions;
using Framework;
using Vereniging;
using Vereniging.Subtypes.Default;
using Vereniging.Subtypes.NietBepaald;
using Vereniging.Subtypes.Subvereniging;

/// <summary>A relatieaanvraag to become subvereniging of the koepel.</summary>
public sealed class Subverenigingsaanvraag : Relatieaanvraag
{
    private Subverenigingsaanvraag(RelatieaanvraagWerdIngediend ingediend)
        : base(ingediend) { }

    public static async Task<Subverenigingsaanvraag> Create(
        Partijen partijen,
        BeoogdeKoepel koepel,
        VerenigingState vereniging,
        string beschrijving,
        OpenRelatieaanvragen openRelatieaanvragen,
        IRelatieaanvraagIdService aanvraagIdService,
        DateOnly vandaag
    )
    {
        CheckRules(koepel, vereniging, openRelatieaanvragen, vandaag);

        return new Subverenigingsaanvraag(
            await Ingediend(partijen, RelatieType.Subvereniging, beschrijving, aanvraagIdService, vandaag)
        );
    }

    /// <summary>
    ///     The rules are checked again, on what may have changed since the relatieaanvraag was ingediend. The vereniging
    ///     becomes subvereniging of the koepel.
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

        vereniging.WordSubverenigingVanKoepel(
            koepel.VCode,
            koepel.Naam,
            SubverenigingIdentificatie.Create(relatiegegevens.Identificatie),
            SubverenigingBeschrijving.Create(relatiegegevens.Beschrijving)
        );
    }

    /// <summary>
    ///     The rules of a subverenigingsaanvraag, in the order of the story.
    ///     <list type="bullet">
    ///         <item>Only a vereniging zonder eigen rechtspersoonlijkheid with subtype leeg or NB.</item>
    ///         <item>K6: no lidmaatschap that has not ended, also not through an open lidmaatschapsaanvraag.</item>
    ///         <item>K7: one open subverenigingsaanvraag per vereniging, with any koepel.</item>
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
        ControleerGeenOpenRelatieaanvraagVoorKoepelEnVereniging(RelatieType.Subvereniging, openRelatieaanvragen);

        Throw<SubverenigingsaanvraagIsNietToegestaan>.IfNot(MagSubverenigingWorden(vereniging));

        Throw<VerenigingMetLidmaatschapKanGeenSubverenigingWorden>.If(
            vereniging.Lidmaatschappen.Any(lidmaatschap => IsNietBeeindigd(lidmaatschap, today))
        );

        if (openRelatieaanvragen.Lidmaatschapsaanvragen.Count > 0)
            throw new VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden(
                openRelatieaanvragen.Lidmaatschapsaanvragen.Select(aanvraag => aanvraag.AanvraagId).ToArray()
            );

        if (openRelatieaanvragen.Subverenigingsaanvraag is { } openSubverenigingsaanvraag)
            throw new SubverenigingsaanvraagStaatAlOpen(openSubverenigingsaanvraag.AanvraagId);
    }

    // Type checks: the subtype of a vereniging met rechtspersoonlijkheid (NoSubtype) throws on its Code.
    private static bool MagSubverenigingWorden(VerenigingState vereniging) =>
        !Verenigingstype.IsKboVereniging(vereniging.Verenigingstype.Code)
        && vereniging.Verenigingssubtype is null or DefaultSubtype or NietBepaaldSubtype;
}
