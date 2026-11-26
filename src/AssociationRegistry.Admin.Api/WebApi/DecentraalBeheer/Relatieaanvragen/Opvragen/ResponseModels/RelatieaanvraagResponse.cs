namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.Opvragen.ResponseModels;

using System.Runtime.Serialization;
using AssociationRegistry.Admin.Schema.Relatieaanvragen;
using Formats;

/// <summary>Een relatieaanvraag tussen een koepel en een vereniging</summary>
[DataContract]
public class RelatieaanvraagResponse
{
    /// <summary>De unieke identificatie van de relatieaanvraag</summary>
    [DataMember(Name = "aanvraagId")]
    public string AanvraagId { get; init; } = null!;

    /// <summary>De vCode van de koepel</summary>
    [DataMember(Name = "koepelVCode")]
    public string KoepelVCode { get; init; } = null!;

    /// <summary>De vCode van de vereniging die lid of subvereniging van de koepel wordt</summary>
    [DataMember(Name = "verenigingVCode")]
    public string VerenigingVCode { get; init; } = null!;

    /// <summary>De vCode van de vereniging die de aanvraag indiende: de koepel of de vereniging</summary>
    [DataMember(Name = "aangevraagdDoor")]
    public string AangevraagdDoor { get; init; } = null!;

    /// <summary>De gevraagde relatie: Lidmaatschap of Subvereniging</summary>
    [DataMember(Name = "relatieType")]
    public string RelatieType { get; init; } = null!;

    /// <summary>De status van de aanvraag: Aangevraagd, Bevestigd, Geweigerd, Ingetrokken of Vervallen</summary>
    [DataMember(Name = "status")]
    public string Status { get; init; } = null!;

    /// <summary>De datum waarop de aanvraag werd ingediend</summary>
    [DataMember(Name = "aanvraagDatum")]
    public string AanvraagDatum { get; init; } = null!;

    /// <summary>De datum waarop de aanvraag vervalt als ze niet afgehandeld is</summary>
    [DataMember(Name = "vervalDatum")]
    public string VervalDatum { get; init; } = null!;

    /// <summary>De beschrijving van de aanvraag</summary>
    [DataMember(Name = "beschrijving")]
    public string Beschrijving { get; init; } = null!;

    /// <summary>
    /// De datum waarop de aanvraag afgehandeld werd: bevestigd, geweigerd, ingetrokken of vervallen. Leeg zolang de
    /// aanvraag open is.
    /// </summary>
    [DataMember(Name = "afhandelDatum")]
    public string AfhandelDatum { get; init; } = null!;

    public static RelatieaanvraagResponse From(RelatieaanvraagDocument document) =>
        new()
        {
            AanvraagId = document.AanvraagId,
            KoepelVCode = document.KoepelVCode,
            VerenigingVCode = document.VerenigingVCode,
            AangevraagdDoor = document.AangevraagdDoor,
            RelatieType = document.RelatieType,
            Status = document.Status,
            AanvraagDatum = document.AanvraagDatum.FormatAsBelgianDate(),
            VervalDatum = document.VervalDatum.FormatAsBelgianDate(),
            Beschrijving = document.Beschrijving,
            AfhandelDatum =
                document.AfhandelDatum == default ? string.Empty : document.AfhandelDatum.FormatAsBelgianDate(),
        };
}

/// <summary>De relatieaanvragen van een vereniging, als koepel of als vereniging</summary>
[DataContract]
public class RelatieaanvragenResponse
{
    /// <summary>De relatieaanvragen</summary>
    [DataMember(Name = "relatieaanvragen")]
    public RelatieaanvraagResponse[] Relatieaanvragen { get; init; } = [];
}
