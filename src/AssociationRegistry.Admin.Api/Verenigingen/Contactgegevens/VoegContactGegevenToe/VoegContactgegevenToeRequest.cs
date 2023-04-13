namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;

using System.Runtime.Serialization;
using AssociationRegistry.Contactgegevens;
using Vereniging.VoegContactgegevenToe;
using Infrastructure.Swagger;
using VCodes;

[DataContract]
public class VoegContactgegevenToeRequest
{
    [DataMember(Name = "initiator")] public string Initiator { get; set; } = null!;

    /// <summary>
    /// Het toe te voegen contactgegeven
    /// </summary>
    [DataMember(Name = "contactgegeven")]
    public RequestContactgegeven Contactgegeven { get; set; } = null!;


    /// <summary>
    /// Het toe te voegen contactgegeven
    /// </summary>
    [DataContract]
    public class RequestContactgegeven
    {
        /// <summary>
        /// Het type contactgegeven.
        /// </summary>
        [SwaggerParameterExample("Email")]
        [SwaggerParameterExample("Socialmedia")]
        [SwaggerParameterExample("Telefoon")]
        [SwaggerParameterExample("Website")]
        [DataMember(Name = "type")]
        public string Type { get; set; } = null!;

        /// <summary>
        /// De waarde van het contactgegeven
        /// </summary>
        [DataMember(Name = "waarde")]
        public string Waarde { get; set; } = null!;

        /// <summary>
        /// Vrij veld die het het contactgegeven omschrijft (bijv: algemeen, administratie, ...)
        /// </summary>
        [DataMember(Name = "omschrijving")]
        public string? Omschrijving { get; set; } = null;

        /// <summary>
        /// Duidt het contactgegeven aan als primair contactgegeven
        /// </summary>
        [DataMember(Name = "isPrimair", EmitDefaultValue = false)]
        public bool IsPrimair { get; set; }
    }

    public VoegContactgegevenToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            AssociationRegistry.Contactgegevens.Contactgegeven.Create(
                ContactgegevenType.Parse(Contactgegeven.Type),
                Contactgegeven.Waarde,
                Contactgegeven.Omschrijving,
                Contactgegeven.IsPrimair));
}
