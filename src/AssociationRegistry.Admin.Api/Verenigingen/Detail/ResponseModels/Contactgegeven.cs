namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>Een contactgegeven van een vereniging</summary>
[DataContract]
public class Contactgegeven
{
    /// <summary>De unieke identificatie code van dit contactgegeven binnen de vereniging</summary>
    [DataMember(Name = "ContactgegevenId")]
    public int ContactgegevenId { get; init; }

    /// <summary>Het type contactgegeven</summary>
    [DataMember(Name = "Type")]
    public string Type { get; init; } = null!;

    /// <summary>De waarde van het contactgegeven</summary>
    [DataMember(Name = "Waarde")]
    public string Waarde { get; init; } = null!;

    /// <summary>
    ///     Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
    /// </summary>
    [DataMember(Name = "Beschrijving")]
    public string? Beschrijving { get; init; }

    /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
    [DataMember(Name = "IsPrimair")]
    public bool IsPrimair { get; init; }

    /// <summary> De bron die dit contactgegeven beheert
    /// <br />
    ///     Mogelijke waarden:<br />
    ///     - Initiator<br />
    ///     - KBO
    /// </summary>
    [DataMember(Name = "Bron")]
    public string Bron { get; init; } = null!;
}
