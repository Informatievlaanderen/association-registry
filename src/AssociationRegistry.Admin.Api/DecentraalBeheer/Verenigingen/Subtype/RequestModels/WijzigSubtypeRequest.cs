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

    public WijzigSubtypeCommand ToCommand(string vCodeAsString, string? andereVerenigingNaam)
    {
        var subtype = AssociationRegistry.Vereniging.Subtype.Parse(Subtype);
        var vCode = VCode.Create(vCodeAsString);

        return subtype.Code switch
        {
            var code when IsFeitelijkeVereniging(code)
                => new WijzigSubtypeCommand(vCode, new WijzigSubtypeCommand.TeWijzigenNaarFeitelijkeVereniging(subtype)),

            var code when IsNogNietBepaald(code)
                => new WijzigSubtypeCommand(vCode, new WijzigSubtypeCommand.TerugTeZettenNaarNogNietBepaald(subtype)),

            var code when IsSubVereniging(code)
                => new WijzigSubtypeCommand(
                    vCode,
                    new WijzigSubtypeCommand.TeWijzigenSubtype(
                        subtype,
                        VCode.Create(AndereVereniging!),
                        andereVerenigingNaam,
                        SubtypeIdentificatie.Create(Identificatie ?? string.Empty),
                        SubtypeBeschrijving.Create(Beschrijving ?? string.Empty))),

            _ => throw new ArgumentException($"Unknown subtype: {Subtype}"),
        };
    }

    private bool IsFeitelijkeVereniging(string code)
        => code == AssociationRegistry.Vereniging.Subtype.FeitelijkeVereniging.Code;

    private bool IsNogNietBepaald(string code)
        => code == AssociationRegistry.Vereniging.Subtype.NogNietBepaald.Code;

    private bool IsSubVereniging(string code)
        => code == AssociationRegistry.Vereniging.Subtype.SubVereniging.Code;
}
