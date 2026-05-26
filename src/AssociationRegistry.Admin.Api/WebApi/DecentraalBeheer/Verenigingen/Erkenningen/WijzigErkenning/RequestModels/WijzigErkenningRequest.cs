namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning.RequestModels;

using System.Runtime.Serialization;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Primitives;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;

[DataContract]
public record WijzigErkenningRequest
{
    /// <summary>
    /// Datum waarop de erkenning hernieuwd kan worden. Een leeg veld (““) wordt geïnterpreteerd als null.
    /// </summary>
    [DataMember(Name = "hernieuwingsdatum")]
    public NullOrEmpty<DateOnly> Hernieuwingsdatum { get; set; }

    /// <summary>
    /// URL voor het hernieuwen van de erkenning.
    /// </summary>
    [DataMember(Name = "hernieuwingsUrl")]
    public string? HernieuwingsUrl { get; set; }

    /// <summary>
    /// Startdatum van de erkenning in formaat YYYY-MM-DD. Een leeg veld (““) wordt geïnterpreteerd als null.
    /// </summary>
    [DataMember(Name = "startDatum")]
    public NullOrEmpty<DateOnly> Startdatum { get; set; }

    /// <summary>
    /// Einddatum van de erkenning in formaat YYYY-MM-DD. Een leeg veld (““) wordt geïnterpreteerd als null.
    /// </summary>
    [DataMember(Name = "eindDatum")]
    public NullOrEmpty<DateOnly> Einddatum { get; set; }

    /// <summary>
    /// WijzigingsType wijziging van de erkenning (Corrigeer, Verlenging)
    /// </summary>
    [DataMember(Name = "type")]
    public string WijgingsType { get; set; }

    public WijzigErkenningCommand ToCommand(string vCode, int erkenningId) =>
        new(
            VCode.Create(vCode),
            DecentraalBeheer.Vereniging.Erkenningen.TeWijzigenErkenning.Create(
                erkenningId,
                Startdatum,
                Einddatum,
                Hernieuwingsdatum,
                HernieuwingsUrl,
                WijgingsType)
        );
}
