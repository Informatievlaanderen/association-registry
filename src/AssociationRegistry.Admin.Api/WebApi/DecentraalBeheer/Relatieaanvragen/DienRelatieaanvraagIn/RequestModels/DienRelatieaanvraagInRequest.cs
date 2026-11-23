namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.DienRelatieaanvraagIn;
using DecentraalBeheer.Vereniging;
using DomainRelatieType = DecentraalBeheer.Relatieaanvragen.RelatieType;
using Partijen = DecentraalBeheer.Relatieaanvragen.Partijen;

[DataContract]
public class DienRelatieaanvraagInRequest
{
    /// <summary>De vCode van de koepel.</summary>
    [DataMember]
    public string KoepelVCode { get; set; } = null!;

    /// <summary>De vCode van de vereniging die lid of subvereniging van de koepel wordt.</summary>
    [DataMember]
    public string VerenigingVCode { get; set; } = null!;

    /// <summary>
    ///     De gevraagde relatie.
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Lidmaatschap<br />
    ///     - Subvereniging<br />
    /// </summary>
    [DataMember]
    public string RelatieType { get; set; } = null!;

    /// <summary>Een beschrijving van de aanvraag (optioneel).</summary>
    [DataMember(EmitDefaultValue = false)]
    public string? Beschrijving { get; set; }

    /// <summary>
    ///     The value objects check the input before anything is loaded: the vCodes, the partijen (vr-namens must be the
    ///     koepel or the vereniging, and they differ) and the relatieType.
    /// </summary>
    public DienRelatieaanvraagInCommand ToCommand(VCode aangevraagdDoor) =>
        new(
            Partijen.Create(VCode.Create(KoepelVCode), VCode.Create(VerenigingVCode), aangevraagdDoor),
            DomainRelatieType.Parse(RelatieType),
            Beschrijving ?? string.Empty
        );
}
