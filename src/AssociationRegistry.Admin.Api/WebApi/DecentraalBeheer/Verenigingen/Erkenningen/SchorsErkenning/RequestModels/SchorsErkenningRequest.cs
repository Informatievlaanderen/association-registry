namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;

[DataContract]
public record SchorsErkenningRequest
{
    /// <summary>
    /// Reden waarom de erkenning geschorst wordt.
    /// </summary>
    [DataMember(Name = "redenSchorsing")]
    public string RedenSchorsing { get; set; }

    public SchorsErkenningCommand ToCommand(string vCode, int erkenningId) =>
        new(
            VCode.Create(vCode),
            new TeSchorsenErkenning { ErkenningId = erkenningId, RedenSchorsing = RedenSchorsing }
        );
}
