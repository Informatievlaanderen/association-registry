namespace AssociationRegistry.Admin.Api.Verenigingen.Subtype;

using System.Runtime.Serialization;

[DataContract]
public class WijzigSubtypeRequest
{
    /// <summary>
    /// De code van het subtype
    /// </summary>
    [DataMember]
    public string Subtype { get; set; }

    /// <summary>
    /// De vCode van de andere vereniging
    /// </summary>
    [DataMember]
    public string AndereVereniging { get; set; }

    /// <summary>
    /// De externe identificatie voor het lidmaatschap
    /// </summary>
    [DataMember]
    public string? Identificatie { get; set; } = string.Empty;

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// </summary>
    [DataMember]
    public string? Beschrijving { get; set; } = string.Empty;

    // public VoegLidmaatschapToeCommand ToCommand(string vCode, string andereVerenigingNaam) => new(
    //     VCode.Create(vCode),
    //     new VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap(
    //         VCode.Create(AndereVereniging),
    //         andereVerenigingNaam,
    //         new Geldigheidsperiode(new GeldigVan(Van), new GeldigTot(Tot)),
    //         LidmaatschapIdentificatie.Create(Identificatie ?? string.Empty),
    //         LidmaatschapBeschrijving.Create(Beschrijving ?? string.Empty)
    //     ));
}
