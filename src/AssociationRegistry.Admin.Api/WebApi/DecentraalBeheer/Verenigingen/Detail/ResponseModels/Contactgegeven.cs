namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

/// <summary>Een contactgegeven van een vereniging</summary>
[DataContract]
public class Contactgegeven
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>De unieke identificatie code van dit contactgegeven binnen de vereniging</summary>
    [DataMember(Name = "ContactgegevenId")]
    public int ContactgegevenId { get; init; }

    /// <summary>Het type contactgegeven</summary>
    [DataMember(Name = "Contactgegeventype")]
    public string Contactgegeventype { get; init; } = null!;

    /// <summary>De waarde van het contactgegeven</summary>
    [DataMember(Name = "Waarde")]
    public string Waarde { get; init; } = null!;

    /// <summary>
    ///     Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
    /// </summary>
    [DataMember(Name = "Beschrijving")]
    [MaxLength(DecentraalBeheer.Vereniging.Contactgegeven.MaxLengthBeschrijving)]
    public string? Beschrijving { get; init; }

    /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
    [DataMember(Name = "IsPrimair")]
    public bool IsPrimair { get; init; }

    /// <summary>
    ///     De bron die dit contactgegeven beheert
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Initiator<br />
    ///     - KBO
    /// </summary>
    [DataMember(Name = "Bron")]
    public string Bron { get; init; } = null!;
}
