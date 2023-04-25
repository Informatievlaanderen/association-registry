namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using System.Runtime.Serialization;
using Infrastructure.Swagger;

/// <summary>
/// Het toe te voegen contactgegeven
/// </summary>
[DataContract]
public class ToeTeVoegenContactgegeven
{
    /// <summary>Het type contactgegeven</summary>
    [SwaggerParameterExample("Email")]
    [SwaggerParameterExample("SocialMedia")]
    [SwaggerParameterExample("Telefoon")]
    [SwaggerParameterExample("Website")]
    [DataMember(Name = "type")]
    public string Type { get; set; } = null!;

    /// <summary>De waarde van het contactgegeven</summary>
    [DataMember(Name = "waarde")]
    public string Waarde { get; set; } = null!;

    /// <summary>
    /// Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
    /// </summary>
    [DataMember(Name = "beschrijving")]
    public string? Beschrijving { get; set; }

    /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
    [DataMember(Name = "isPrimair")]
    public bool IsPrimair { get; set; }
}
