namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.WeigerRelatieaanvraag;
using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Vereniging;

/// <summary>De weigering van de relatieaanvraag.</summary>
[DataContract]
public class WeigerRelatieaanvraagRequest
{
    /// <summary>De reden van de weigering (optioneel).</summary>
    [DataMember(EmitDefaultValue = false)]
    public string? Reden { get; set; }

    /// <summary>The value object checks the aanvraagId from the route before anything is loaded.</summary>
    public WeigerRelatieaanvraagCommand ToCommand(string aanvraagId, VCode geweigerdDoor) =>
        new(RelatieaanvraagId.Parse(aanvraagId), geweigerdDoor, Reden ?? string.Empty);
}
