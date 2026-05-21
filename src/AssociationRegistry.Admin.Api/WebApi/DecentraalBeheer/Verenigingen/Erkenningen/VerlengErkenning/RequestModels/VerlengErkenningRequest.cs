namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.VerlengErkenning.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerlengErkenning;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Primitives;

[DataContract]
public record VerlengErkenningRequest
{
    /// <summary>
    /// Einddatum van de erkenning in formaat YYYY-MM-DD. Een leeg veld (““) wordt geïnterpreteerd als null.
    /// </summary>
    [DataMember(Name = "einddatum")]
    public DateOnly Einddatum { get; set; }

    /// <summary>
    /// Datum waarop de erkenning hernieuwd kan worden. Een leeg veld (““) wordt geïnterpreteerd als null.
    /// </summary>
    [DataMember(Name = "hernieuwingsdatum")]
    public NullOrEmpty<DateOnly> Hernieuwingsdatum { get; set; }

    public VerlengErkenningCommand ToCommand(string vCode, int erkenningId, DateOnly einddatum, NullOrEmpty<DateOnly> hernieuwingsdatum) =>
        new(
            VCode.Create(vCode),
            new TeVerlengenErkenning
            {
                ErkenningId = erkenningId,
                Einddatum = einddatum,
                Hernieuwingsdatum = hernieuwingsdatum,

            }
        );
}
