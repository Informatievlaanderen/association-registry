namespace AssociationRegistry.MartenDb.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Vereniging;
using JasperFx;
using DomainRelatieType = AssociationRegistry.DecentraalBeheer.Relatieaanvragen.RelatieType;

/// <summary>
///     Write side guard, saved in the same transaction as the events of the relatieaanvraag. Its id is the key that may
///     have only one open relatieaanvraag: a second insert with the same id fails in the database.
///     Must be deleted when the relatieaanvraag is closed (bevestigd, geweigerd, ingetrokken or vervallen).
/// </summary>
public class OpenRelatieaanvraagDocument
{
    [Identity]
    public string Sleutel { get; set; } = null!;

    public string AanvraagId { get; set; } = null!;
    public string RelatieType { get; set; } = null!;

    /// <summary>To find the open lidmaatschapsaanvragen of a vereniging (K6); there can be one per koepel.</summary>
    public string VerenigingVCode { get; set; } = null!;

    public static string SleutelVoorKoepelEnVereniging(VCode koepelVCode, VCode verenigingVCode) =>
        $"{koepelVCode.Value}-{verenigingVCode.Value}";

    public static string SleutelVoorSubverenigingsaanvraag(VCode verenigingVCode) =>
        $"{DomainRelatieType.Subvereniging.Value}-{verenigingVCode.Value}";

    public static IEnumerable<OpenRelatieaanvraagDocument> VoorNieuweAanvraag(Relatieaanvraag relatieaanvraag)
    {
        yield return Create(
            SleutelVoorKoepelEnVereniging(relatieaanvraag.KoepelVCode, relatieaanvraag.VerenigingVCode),
            relatieaanvraag
        );

        if (relatieaanvraag.RelatieType.IsSubvereniging)
            yield return Create(SleutelVoorSubverenigingsaanvraag(relatieaanvraag.VerenigingVCode), relatieaanvraag);
    }

    public OpenRelatieaanvraag ToOpenRelatieaanvraag() =>
        new(RelatieaanvraagId.Hydrate(AanvraagId), DomainRelatieType.Parse(RelatieType));

    private static OpenRelatieaanvraagDocument Create(string sleutel, Relatieaanvraag relatieaanvraag) =>
        new()
        {
            Sleutel = sleutel,
            AanvraagId = relatieaanvraag.AanvraagId.Value,
            RelatieType = relatieaanvraag.RelatieType.Value,
            VerenigingVCode = relatieaanvraag.VerenigingVCode.Value,
        };
}
