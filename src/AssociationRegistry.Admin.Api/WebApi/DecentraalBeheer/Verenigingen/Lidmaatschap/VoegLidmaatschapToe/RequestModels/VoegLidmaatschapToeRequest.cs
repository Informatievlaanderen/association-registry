namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;

using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;
using DecentraalBeheer.Vereniging;
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
    public string? Identificatie { get; set; } = string.Empty;

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// </summary>
    [DataMember]
    public string? Beschrijving { get; set; } = string.Empty;

    public VoegLidmaatschapToeCommand ToCommand(string vCode, string andereVerenigingNaam) => new(
        VCode.Create(vCode),
        new ToeTeVoegenLidmaatschap(
        VCode.Create(AndereVereniging),
        andereVerenigingNaam,
        new Geldigheidsperiode(new GeldigVan(Van), new GeldigTot(Tot)),
        LidmaatschapIdentificatie.Create(Identificatie ?? string.Empty),
        LidmaatschapBeschrijving.Create(Beschrijving ?? string.Empty)
    ));
}
