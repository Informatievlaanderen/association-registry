namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;

using System.Runtime.Serialization;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerSchorsingErkenning;

[DataContract]
public record CorrigeerSchorsingErkenningRequest
{
    /// <summary>
    /// Reden waarom de erkenning geschorst wordt.
    /// </summary>
    [DataMember(Name = "redenSchorsing")]
    public string RedenSchorsing { get; set; }

    public CorrigeerSchorsingErkenningCommand ToCommand(string vCode, int erkenningId) =>
        new(
            VCode.Create(vCode),
            new DecentraalBeheer.Vereniging.Erkenningen.TeCorrigerenSchorsingErkenning
            {
                ErkenningId = erkenningId,
                RedenSchorsing = RedenSchorsing,
            }
        );
}
