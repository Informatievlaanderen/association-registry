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

    /// <summary>De bevestiging van de aanvraag. Ontbreekt zolang de aanvraag niet bevestigd is.</summary>
    [DataMember(Name = "bevestiging", EmitDefaultValue = false)]
    public RelatieaanvraagBevestigingResponse? Bevestiging { get; init; }

    /// <summary>De weigering van de aanvraag. Ontbreekt zolang de aanvraag niet geweigerd is.</summary>
    [DataMember(Name = "weigering", EmitDefaultValue = false)]
    public RelatieaanvraagWeigeringResponse? Weigering { get; init; }

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
            Bevestiging = document.Bevestiging is { } bevestiging
                ? new RelatieaanvraagBevestigingResponse { Datum = bevestiging.Datum.FormatAsBelgianDate() }
                : null,
            Weigering = document.Weigering is { } weigering
                ? new RelatieaanvraagWeigeringResponse
                {
                    Datum = weigering.Datum.FormatAsBelgianDate(),
                    Reden = weigering.Reden,
                }
                : null,
        };
}

/// <summary>De bevestiging van een relatieaanvraag</summary>
[DataContract]
public class RelatieaanvraagBevestigingResponse
{
    /// <summary>De datum waarop de aanvraag bevestigd werd</summary>
    [DataMember(Name = "datum")]
    public string Datum { get; init; } = null!;
}

/// <summary>De weigering van een relatieaanvraag</summary>
[DataContract]
public class RelatieaanvraagWeigeringResponse
{
    /// <summary>De datum waarop de aanvraag geweigerd werd</summary>
    [DataMember(Name = "datum")]
    public string Datum { get; init; } = null!;

    /// <summary>De reden van de weigering. Leeg als er geen reden opgegeven werd.</summary>
    [DataMember(Name = "reden")]
    public string Reden { get; init; } = null!;
}

/// <summary>De relatieaanvragen van een vereniging, als koepel of als vereniging</summary>
[DataContract]
public class RelatieaanvragenResponse
{
    /// <summary>De relatieaanvragen</summary>
    [DataMember(Name = "relatieaanvragen")]
    public RelatieaanvraagResponse[] Relatieaanvragen { get; init; } = [];
}
