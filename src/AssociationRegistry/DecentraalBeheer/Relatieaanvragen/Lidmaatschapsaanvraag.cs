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

    /// <summary>
    ///     The rules of a lidmaatschapsaanvraag, in the order of the story.
    ///     <list type="bullet">
    ///         <item>A new lidmaatschap starts on confirmation without einddatum: every lidmaatschap with the koepel that has not ended blocks it.</item>
    ///         <item>K6: a subvereniging cannot become lid, also not through an open subverenigingsaanvraag.</item>
    ///     </list>
    /// </summary>
    public static async Task<Lidmaatschapsaanvraag> Create(
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

        Throw<ActueelOfToekomstigLidmaatschapBijDezelfdeKoepel>.If(
            LidmaatschappenBij(vereniging, koepel).Any(lidmaatschap => IsNietBeeindigd(lidmaatschap, vandaag))
        );

        ControleerGeenOpenRelatieaanvraagVoorKoepelEnVereniging(RelatieType.Lidmaatschap, openRelatieaanvragen);

        Throw<SubverenigingKanGeenLidWorden>.If(vereniging.Verenigingssubtype is SubverenigingSubtype);

        if (openRelatieaanvragen.Subverenigingsaanvraag is { } openSubverenigingsaanvraag)
            throw new VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden(openSubverenigingsaanvraag.AanvraagId);

        return new Lidmaatschapsaanvraag(
            await Ingediend(partijen, RelatieType.Lidmaatschap, beschrijving, aanvraagIdService, vandaag)
        );
    }
}
