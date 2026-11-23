namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Events;
using Exceptions;
using Framework;
using Vereniging;
using Vereniging.Subtypes.Default;
using Vereniging.Subtypes.NietBepaald;

/// <summary>A relatieaanvraag to become subvereniging of the koepel.</summary>
public sealed class Subverenigingsaanvraag : Relatieaanvraag
{
    private Subverenigingsaanvraag(RelatieaanvraagWerdIngediend ingediend)
        : base(ingediend) { }

    /// <summary>
    ///     The rules of a subverenigingsaanvraag, in the order of the story.
    ///     <list type="bullet">
    ///         <item>Only a vereniging zonder eigen rechtspersoonlijkheid with subtype leeg or NB.</item>
    ///         <item>K6: no lidmaatschap that has not ended, also not through an open lidmaatschapsaanvraag.</item>
    ///         <item>K7: one open subverenigingsaanvraag per vereniging, with any koepel.</item>
    ///     </list>
    /// </summary>
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
        ControleerGeenActieveRelatie(vereniging, koepel, vandaag);
        ControleerGeenOpenRelatieaanvraagVoorKoepelEnVereniging(RelatieType.Subvereniging, openRelatieaanvragen);

        Throw<SubverenigingsaanvraagIsNietToegestaan>.IfNot(MagSubverenigingWorden(vereniging));

        Throw<VerenigingMetLidmaatschapKanGeenSubverenigingWorden>.If(
            vereniging.Lidmaatschappen.Any(lidmaatschap => IsNietBeeindigd(lidmaatschap, vandaag))
        );

        if (openRelatieaanvragen.Lidmaatschapsaanvragen.Count > 0)
            throw new VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden(
                openRelatieaanvragen.Lidmaatschapsaanvragen.Select(aanvraag => aanvraag.AanvraagId).ToArray()
            );

        if (openRelatieaanvragen.Subverenigingsaanvraag is { } openSubverenigingsaanvraag)
            throw new SubverenigingsaanvraagStaatAlOpen(openSubverenigingsaanvraag.AanvraagId);

        return new Subverenigingsaanvraag(
            await Ingediend(partijen, RelatieType.Subvereniging, beschrijving, aanvraagIdService, vandaag)
        );
    }

    // Type checks: the subtype of a vereniging met rechtspersoonlijkheid (NoSubtype) throws on its Code.
    private static bool MagSubverenigingWorden(VerenigingState vereniging) =>
        !Verenigingstype.IsKboVereniging(vereniging.Verenigingstype.Code)
        && vereniging.Verenigingssubtype is null or DefaultSubtype or NietBepaaldSubtype;
}
