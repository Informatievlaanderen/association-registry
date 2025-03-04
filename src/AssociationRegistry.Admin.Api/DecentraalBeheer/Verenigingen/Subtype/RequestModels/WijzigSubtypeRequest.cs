namespace AssociationRegistry.Admin.Api.Verenigingen.Subtype;

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

    // TODO: refactor
    public WijzigSubtypeCommand ToCommand(string vCode, string? andereVerenigingNaam)
    {
        var subtype = AssociationRegistry.Vereniging.Subtype.Parse(Subtype);

        return subtype.Code switch
        {
            var code when code == AssociationRegistry.Vereniging.Subtype.FeitelijkeVereniging.Code
                => new WijzigSubtypeCommand(
                    VCode.Create(vCode),
                    new WijzigSubtypeCommand.TeWijzigenNaarFeitelijkeVereniging(
                        AssociationRegistry.Vereniging.Subtype.Parse(Subtype))),

            var code when code == AssociationRegistry.Vereniging.Subtype.NogNietBepaald.Code
                => new WijzigSubtypeCommand(
                    VCode.Create(vCode),
                    new WijzigSubtypeCommand.TerugTeZettenNaarNogNietBepaald(
                        AssociationRegistry.Vereniging.Subtype.Parse(Subtype))),

            var code when code == AssociationRegistry.Vereniging.Subtype.SubVereniging.Code
                => new WijzigSubtypeCommand(
                    VCode.Create(vCode),
                    new WijzigSubtypeCommand.TeWijzigenSubtype(
                        AssociationRegistry.Vereniging.Subtype.Parse(Subtype),
                        VCode.Create(AndereVereniging!),
                        andereVerenigingNaam,
                        SubtypeIdentificatie.Create(Identificatie ?? string.Empty),
                        SubtypeBeschrijving.Create(Beschrijving ?? string.Empty))),

            _ => throw new ArgumentException($"Unknown subtype: {Subtype}"),
        };
    }
}
