namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.BevestigRelatieaanvraag;
using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Vereniging;

/// <summary>De gegevens van de relatie die bij de bevestiging ontstaat. Alle velden zijn optioneel.</summary>
[DataContract]
public class BevestigRelatieaanvraagRequest
{
    /// <summary>De externe identificatie van het lidmaatschap of de subvereniging (optioneel).</summary>
    [DataMember(EmitDefaultValue = false)]
    public string? Identificatie { get; set; }

    /// <summary>De externe beschrijving van het lidmaatschap of de subvereniging (optioneel).</summary>
    [DataMember(EmitDefaultValue = false)]
    public string? Beschrijving { get; set; }

    /// <summary>The value object checks the aanvraagId from the route before anything is loaded.</summary>
    public BevestigRelatieaanvraagCommand ToCommand(string aanvraagId, VCode bevestigdDoor) =>
        new(RelatieaanvraagId.Parse(aanvraagId), bevestigdDoor, Relatiegegevens.Create(Identificatie, Beschrijving));
}
