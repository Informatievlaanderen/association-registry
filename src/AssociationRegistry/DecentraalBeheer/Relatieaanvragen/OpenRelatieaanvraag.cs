namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

public record OpenRelatieaanvraag(RelatieaanvraagId AanvraagId, RelatieType RelatieType);

/// <summary>
///     The open relatieaanvragen that can block a new one: the one for the same koepel and vereniging, and the open
///     subverenigingsaanvraag and lidmaatschapsaanvragen of the vereniging (with any koepel).
/// </summary>
public record OpenRelatieaanvragen(
    OpenRelatieaanvraag? VoorKoepelEnVereniging,
    OpenRelatieaanvraag? Subverenigingsaanvraag,
    IReadOnlyCollection<OpenRelatieaanvraag> Lidmaatschapsaanvragen
)
{
    public static OpenRelatieaanvragen Geen =>
        new(VoorKoepelEnVereniging: null, Subverenigingsaanvraag: null, Lidmaatschapsaanvragen: []);
}
