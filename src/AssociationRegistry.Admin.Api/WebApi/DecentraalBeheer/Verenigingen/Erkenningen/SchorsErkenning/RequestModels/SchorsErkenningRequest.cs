namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using DecentraalBeheer.Vereniging;

[DataContract]
public record SchorsErkenningRequest
{
    /// <summary>De te schorsen erkenning</summary>
    [DataMember(Name = "Erkenning")]
    public TeSchorsenErkenning Erkenning { get; set; } = null!;

    public SchorsErkenningCommand ToCommand(string vCode, int erkenningId)
    {

        return new SchorsErkenningCommand(
            VCode.Create(vCode),
            new DecentraalBeheer.Vereniging.Erkenningen.TeSchorsenErkenning
            {
                ErkenningId = erkenningId,
                RedenSchorsing = Erkenning.RedenSchorsing,
            }
        );
    }
}
