namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.RequestModels;

using AssociationRegistry.Acties.VoegLidmaatschapToe;
using AssociationRegistry.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class VoegLidmaatschapToeRequest
{
    /// <summary>
    /// De vCode van de andere vereniging
    /// </summary>
    [DataMember]
    public string AndereVereniging { get; set; }

    /// <summary>
    /// De datum waarop de relatie actief wordt
    /// </summary>
    [DataMember]
    public DateOnly? DatumVan { get; set; }

    /// <summary>
    /// De datum waarop de relatie niet meer actief wordt
    /// </summary>
    [DataMember]
    public DateOnly? DatumTot { get; set; }

    /// <summary>
    /// De externe identificatie voor de relatie
    /// </summary>
    [DataMember]
    public string Identificatie { get; set; }

    /// <summary>
    /// De externe beschrijving van de relatie
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; }

    public VoegLidmaatschapToeCommand ToCommand(string vCode) => new(
        VCode: VCode.Create(vCode),
        Lidmaatschap.Create(VCode.Create(AndereVereniging),
                            new Geldigheidsperiode(new GeldigVan(DatumVan), new GeldigTot(DatumTot)),
                            Identificatie,
                            Beschrijving
        )
    );
}
