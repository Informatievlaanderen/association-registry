namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerRedenSchorsingErkenning.RequestModels;

using System.Runtime.Serialization;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerSchorsingErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging;

[DataContract]
public record CorrigeerRedenSchorsingErkenningRequest
{
    /// <summary>
    /// Gecorrigeerde reden van de schorsing.
    /// </summary>
    [DataMember(Name = "redenSchorsing")]
    public string RedenSchorsing { get; set; } = null!;

    public CorrigeerRedenSchorsingErkenningCommand ToCommand(string vCode, int erkenningId) =>
        new(
            VCode.Create(vCode),
            new DecentraalBeheer.Vereniging.Erkenningen.TeCorrigerenRedenSchorsingErkenning
            {
                ErkenningId = erkenningId,
                RedenSchorsing = RedenSchorsing,
            }
        );
}
