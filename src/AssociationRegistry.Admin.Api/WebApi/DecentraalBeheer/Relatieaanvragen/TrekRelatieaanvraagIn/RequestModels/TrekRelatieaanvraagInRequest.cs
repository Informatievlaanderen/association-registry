namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.TrekRelatieaanvraagIn;
using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Vereniging;

/// <summary>De intrekking van de relatieaanvraag.</summary>
[DataContract]
public class TrekRelatieaanvraagInRequest
{
    /// <summary>De reden van de intrekking (optioneel).</summary>
    [DataMember(EmitDefaultValue = false)]
    public string? Reden { get; set; }

    /// <summary>The value object checks the aanvraagId from the route before anything is loaded.</summary>
    public TrekRelatieaanvraagInCommand ToCommand(string aanvraagId, VCode ingetrokkenDoor) =>
        new(RelatieaanvraagId.Parse(aanvraagId), ingetrokkenDoor, Reden ?? string.Empty);
}
