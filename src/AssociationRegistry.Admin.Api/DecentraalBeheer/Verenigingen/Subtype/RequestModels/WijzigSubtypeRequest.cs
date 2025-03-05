namespace AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;

using DecentraalBeheer.Subtype;
using System.Runtime.Serialization;
using Vereniging;

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
    public string? AndereVereniging { get; set; }

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

    public VerfijnSubtypeNaarFeitelijkeVerenigingCommand ToVerfijnSubtypeNaarFeitelijkeVerenigingCommand(string vCodeAsString)
        => new(VCode.Create(vCodeAsString));

    public ZetSubtypeTerugNaarNogNietBepaaldCommand ToZetSubtypeTerugNaarNogNietBepaaldCommand(string vCodeAsString)
        => new(VCode.Create(vCodeAsString));

    public WijzigSubtypeCommand ToWijzigSubtypeCommand(string vCodeAsString, string? andereVerenigingNaam)
        => new(
            VCode.Create(vCodeAsString),
            new WijzigSubtypeCommand.TeWijzigenSubtype(
                VCode.Create(AndereVereniging!),
                andereVerenigingNaam,
                SubtypeIdentificatie.Create(Identificatie ?? string.Empty),
                SubtypeBeschrijving.Create(Beschrijving ?? string.Empty)));

    private bool IsFeitelijkeVereniging(string code)
        => code == AssociationRegistry.Vereniging.Subtype.FeitelijkeVereniging.Code;

    private bool IsNogNietBepaald(string code)
        => code == AssociationRegistry.Vereniging.Subtype.NogNietBepaald.Code;

    private bool IsSubVereniging(string code)
        => code == AssociationRegistry.Vereniging.Subtype.SubVereniging.Code;
}
