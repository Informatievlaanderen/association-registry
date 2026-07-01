namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Primitives;

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
    /// Reden waarom de erkenning gewijzigd wordt.
    /// </summary>
    [DataMember(Name = "redenVanWijziging")]
    public string RedenVanWijziging { get; set; } = null!;

    /// <summary>
    /// Startdatum van de erkenning in formaat YYYY-MM-DD. Een leeg veld (““) wordt geïnterpreteerd als null.
    /// </summary>
    [DataMember(Name = "startdatum")]
    public NullOrEmpty<DateOnly> Startdatum { get; set; }

    /// <summary>
    /// Einddatum van de erkenning in formaat YYYY-MM-DD. Een leeg veld (““) wordt geïnterpreteerd als null.
    /// </summary>
    [DataMember(Name = "einddatum")]
    public NullOrEmpty<DateOnly> Einddatum { get; set; }

    public WijzigErkenningCommand ToCommand(string vCode, int erkenningId) =>
        new(
            VCode.Create(vCode),
            TeWijzigenErkenning.Create(
                erkenningId,
                Startdatum,
                Einddatum,
                Hernieuwingsdatum,
                HernieuwingsUrl,
                RedenVanWijziging
            )
        );
}
