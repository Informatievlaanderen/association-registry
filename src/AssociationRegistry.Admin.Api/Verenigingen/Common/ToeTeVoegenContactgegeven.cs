namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using Infrastructure.Swagger;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Vereniging;

/// <summary>
///     Het toe te voegen contactgegeven
/// </summary>
[DataContract]
public class ToeTeVoegenContactgegeven
{
    /// <summary>Het type contactgegeven</summary>
    [SwaggerParameterExample("E-mail")]
    [SwaggerParameterExample("SocialMedia")]
    [SwaggerParameterExample("Telefoon")]
    [SwaggerParameterExample("Website")]
    [DataMember(Name = "contactgegeventype")]
    public string Contactgegeventype { get; set; } = null!;

    /// <summary>De waarde van het contactgegeven</summary>
    [DataMember(Name = "waarde")]
    public string Waarde { get; set; } = null!;

    /// <summary>
    ///     Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
    /// </summary>
    [DataMember(Name = "beschrijving")]
    [MaxLength(Contactgegeven.MaxLengthBeschrijving)]
    public string? Beschrijving { get; set; }

    /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
    [DataMember(Name = "isPrimair")]
    public bool IsPrimair { get; set; }

    public static Contactgegeven Map(ToeTeVoegenContactgegeven toeTeVoegenContactgegeven)
        => Contactgegeven.CreateFromInitiator(
            AssociationRegistry.Vereniging.Contactgegeventype.Parse(toeTeVoegenContactgegeven.Contactgegeventype),
            toeTeVoegenContactgegeven.Waarde,
            toeTeVoegenContactgegeven.Beschrijving,
            toeTeVoegenContactgegeven.IsPrimair);
}
