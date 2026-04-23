namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;

using System.Runtime.Serialization;

[DataContract]
public record TeRegistrerenErkenning
{
    /// <summary>
    /// Unieke identificatie van het IPDC-product waarvoor de erkenning wordt toegekend.
    /// </summary>
    [DataMember(Name = "ipdcProductNummer")]
    public string IpdcProductNummer { get; set; } = null!;

    /// <summary>
    /// Startdatum van de erkenning. Bepaalt wanneer de erkenning ingaat.
    /// </summary>
    [DataMember(Name = "Startdatum")]
    public DateOnly? Startdatum { get; set; }

    /// <summary>
    /// Einddatum van de erkenning. Moet groter of gelijk zijn aan de startdatum indien ingevuld.
    /// </summary>
    [DataMember(Name = "Einddatum")]
    public DateOnly? Einddatum { get; set; }

    /// <summary>
    /// Datum waarop de erkenning hernieuwd kan worden. Moet tussen startdatum en einddatum liggen.
    /// </summary>
    [DataMember(Name = "Hernieuwingsdatum")]
    public DateOnly? Hernieuwingsdatum { get; set; }

    /// <summary>
    /// URL voor het hernieuwen van de erkenning.
    /// </summary>
    [DataMember(Name = "HernieuwingsUrl")]
    public string HernieuwingsUrl { get; set; } = null!;
}
