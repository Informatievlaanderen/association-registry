namespace AssociationRegistry.Admin.Api.Verenigingen.Subvereniging.MaakSubvereniging.RequestModels;

using System.Runtime.Serialization;

[DataContract]
public class MaakSubverenigingRequest
{
    /// <summary>
    /// De vCode van de andere vereniging
    /// </summary>
    [DataMember]
    public string AndereVereniging { get; set; }

    /// <summary>
    /// De externe identificatie voor het lidmaatschap
    /// </summary>
    [DataMember]
    public string Identificatie { get; set; }

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; }

    // public KoppelVerenigingCommand ToCommand(string vCode) => new(
    //     VCode: VCode.Create(vCode),
    //     Lidmaatschap.Create(VCode.Create(AndereVereniging),
    //                         new Geldigheidsperiode(new GeldigVan(Van), new GeldigTot(Tot)),
    //                         Identificatie,
    //                         Beschrijving
    //     )
    // );
}
