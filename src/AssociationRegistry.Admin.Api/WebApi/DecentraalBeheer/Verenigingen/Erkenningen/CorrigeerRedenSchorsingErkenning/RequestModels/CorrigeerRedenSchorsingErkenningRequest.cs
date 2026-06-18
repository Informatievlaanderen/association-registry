namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerRedenSchorsingErkenning.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerRedenSchorsingErkenning;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;

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
            new TeCorrigerenRedenSchorsingErkenning
            {
                ErkenningId = erkenningId,
                RedenSchorsing = RedenSchorsing,
            }
        );
}
