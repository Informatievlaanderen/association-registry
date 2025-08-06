namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;

using AssociationRegistry.DecentraalBeheer.Lidmaatschappen.WijzigLidmaatschap;
using AssociationRegistry.Primitives;
using AssociationRegistry.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class WijzigLidmaatschapRequest
{
    /// <summary>
    /// De datum waarop de relatie actief wordt
    /// </summary>
    [DataMember]
    public NullOrEmpty<DateOnly> Van { get; set; }

    /// <summary>
    /// De datum waarop de relatie niet meer actief wordt
    /// </summary>
    [DataMember]
    public NullOrEmpty<DateOnly> Tot { get; set; }

    /// <summary>
    /// De externe identificatie voor het lidmaatschap
    /// </summary>
    [DataMember]
    public string? Identificatie { get; set; }

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// </summary>
    [DataMember]
    public string? Beschrijving { get; set; }

    public WijzigLidmaatschapCommand ToCommand(string vCode, int lidmaatschapId) => new(
        VCode.Create(vCode),
        new WijzigLidmaatschapCommand.TeWijzigenLidmaatschap(
            new LidmaatschapId(lidmaatschapId),
            GeldigVan.FromNullOrEmpty(Van),
            GeldigTot.FromNullOrEmpty(Tot),
            Identificatie is null ? null : LidmaatschapIdentificatie.Create(Identificatie),
            Beschrijving is null ? null : LidmaatschapBeschrijving.Create(Beschrijving)
        ));
}
