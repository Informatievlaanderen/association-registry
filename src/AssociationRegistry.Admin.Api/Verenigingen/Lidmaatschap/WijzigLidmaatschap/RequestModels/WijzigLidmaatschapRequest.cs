namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;

using Acties.VoegLidmaatschapToe;
using Acties.WijzigLidmaatschap;
using System.Runtime.Serialization;
using Vereniging;

[DataContract]
public class WijzigLidmaatschapRequest
{
    /// <summary>
    /// De datum waarop de relatie actief wordt
    /// </summary>
    [DataMember]
    public DateOnly? Van { get; set; }

    /// <summary>
    /// De datum waarop de relatie niet meer actief wordt
    /// </summary>
    [DataMember]
    public DateOnly? Tot { get; set; }

    /// <summary>
    /// De externe identificatie voor het lidmaatschap
    /// </summary>
    [DataMember]
    public string Identificatie { get; set; } = string.Empty;

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; } = string.Empty;

    public WijzigLidmaatschapCommand ToCommand(string vCode, int lidmaatschapId) => new(
        VCode.Create(vCode),
        new WijzigLidmaatschapCommand.TeWijzigenLidmaatschap(
            new LidmaatschapId(lidmaatschapId),
            new Geldigheidsperiode(new GeldigVan(Van), new GeldigTot(Tot)),
            LidmaatschapIdentificatie.Create(Identificatie),
            LidmaatschapBeschrijving.Create(Beschrijving)
        ));
}
