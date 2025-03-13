namespace AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;

using DecentraalBeheer.Subtype;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Vereniging;

[DataContract]
public class WijzigSubtypeRequest
{
    /// <summary>
    /// De code van het subtype vereniging
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - FV<br />
    ///     - NB<br />
    ///     - SUB<br />
    /// </summary>
    [DataMember]
    public string Subtype { get; set; }

    /// <summary>
    /// De vCode van de andere vereniging.
    /// <para>Enkel verplicht bij het verfijnen naar subvereniging.</para>
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public string? AndereVereniging { get; set; }

    /// <summary>
    /// De externe identificatie voor het lidmaatschap
    /// <para>Enkel verplicht bij het verfijnen naar subvereniging.</para>
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public string? Identificatie { get; set; }

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// <para>Enkel verplicht bij het verfijnen naar subvereniging.</para>
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public string? Beschrijving { get; set; }

    public VerfijnSubtypeNaarFeitelijkeVerenigingCommand ToVerfijnSubtypeNaarFeitelijkeVerenigingCommand(string vCodeAsString)
        => new(VCode.Create(vCodeAsString));

    public ZetSubtypeTerugNaarNietBepaaldCommand ToZetSubtypeTerugNaarNietBepaaldCommand(string vCodeAsString)
        => new(VCode.Create(vCodeAsString));

    public VerfijnSubtypeNaarSubverenigingCommand ToWijzigSubtypeCommand(string vCodeAsString, string? andereVerenigingNaam)
        => new(
            VCode.Create(vCodeAsString),
            new VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan(
                VCode.Create(AndereVereniging!),
                andereVerenigingNaam,
                Identificatie is null ? null : SubtypeIdentificatie.Create(Identificatie),
                Beschrijving is null ? null : SubtypeBeschrijving.Create(Beschrijving)));
}
