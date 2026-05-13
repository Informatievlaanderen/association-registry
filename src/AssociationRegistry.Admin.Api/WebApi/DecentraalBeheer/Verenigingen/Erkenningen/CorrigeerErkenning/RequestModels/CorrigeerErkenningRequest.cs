namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerErkenning.RequestModels;

using System.Runtime.Serialization;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerSchorsingErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerErkenning;
using Primitives;

[DataContract]
public record CorrigeerErkenningRequest
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

    public CorrigeerErkenningCommand ToCommand(string vCode, int erkenningId) =>
        new(
            VCode.Create(vCode),
            DecentraalBeheer.Vereniging.Erkenningen.TeCorrigerenErkenning.Create(
                erkenningId,
                Startdatum,
                Einddatum,
                Hernieuwingsdatum,
                HernieuwingsUrl)
        );
}
