namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Events;
using Exceptions;
using Framework;
using Vereniging;

/// <summary>
///     A relatieaanvraag that its ontvanger (the other partij than the aanvrager) can still confirm: it is Aangevraagd
///     and its vervaldatum is not over. Confirming on the vervaldatum itself is still possible.
/// </summary>
public sealed class OntvangenRelatieaanvraag
{
    private readonly RelatieaanvraagState _relatieaanvraag;

    private OntvangenRelatieaanvraag(RelatieaanvraagState relatieaanvraag)
    {
        _relatieaanvraag = relatieaanvraag;
    }

    /// <param name="namens">The vCode from vr-namens: must be the ontvanger.</param>
    public static OntvangenRelatieaanvraag Create(RelatieaanvraagState relatieaanvraag, VCode namens, DateOnly today)
    {
        Throw<NamensIsNietDeOntvanger>.If(namens != relatieaanvraag.Ontvanger);
        Throw<RelatieaanvraagWerdAlBevestigd>.If(relatieaanvraag.Status == RelatieaanvraagStatus.Bevestigd);
        Throw<RelatieaanvraagIsVervallen>.If(today > relatieaanvraag.VervalDatum);

        return new OntvangenRelatieaanvraag(relatieaanvraag);
    }

    /// <summary>
    ///     Checks the koepel and the rules of the type of relatieaanvraag again, and adds the relatie to the vereniging.
    ///     The caller saves the event and the vereniging in one transaction.
    /// </summary>
    public RelatieaanvraagWerdBevestigd Bevestig(
        VerenigingsBase koepel,
        VerenigingOfAnyKind vereniging,
        OpenRelatieaanvragen openRelatieaanvragen,
        Relatiegegevens relatiegegevens,
        DateOnly today
    )
    {
        var beoogdeKoepel = BeoogdeKoepel.Create(koepel.CurrentState);
        var others = openRelatieaanvragen.Without(_relatieaanvraag.AanvraagId);

        if (_relatieaanvraag.RelatieType.IsSubvereniging)
            Subverenigingsaanvraag.Bevestig(beoogdeKoepel, vereniging, others, relatiegegevens, today);
        else
            Lidmaatschapsaanvraag.Bevestig(beoogdeKoepel, vereniging, others, relatiegegevens, today);

        return new RelatieaanvraagWerdBevestigd(_relatieaanvraag.AanvraagId.Value, today);
    }
}
