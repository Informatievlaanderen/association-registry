namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;

using Acties.VoegLidmaatschapToe;
using System.Runtime.Serialization;
using Vereniging;

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
    public string Identificatie { get; set; }

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; }

    public VoegLidmaatschapToeCommand ToCommand(string vCode) => new(
        VCode: VCode.Create(vCode),
        Lidmaatschap.Create(VCode.Create(AndereVereniging),
                            new Geldigheidsperiode(new GeldigVan(Van), new GeldigTot(Tot)),
                            Identificatie,
                            Beschrijving
        )
    );
}
