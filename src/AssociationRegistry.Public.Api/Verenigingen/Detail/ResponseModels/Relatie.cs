namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Relatie
{
    /// <summary>
    /// Het type relatie
    /// </summary>
    [DataMember(Name = "Type")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// De andere vereniging
    /// </summary>
    [DataMember(Name = "AndereVereniging")]
    public GerelateerdeVereniging AndereVereniging { get; set; } = null!;

    [DataContract]
    public class GerelateerdeVereniging
    {
        /// <summary>
        /// De unieke identificator van de andere vereniging in een externe bron
        /// </summary>
        [DataMember(Name = "ExternId")]
        public string ExternId { get; set; } = null!;

        /// <summary>
        /// De unieke identificator van de andere vereniging in het verenigingsregister
        /// </summary>
        [DataMember(Name = "VCode")]
        public string VCode { get; set; } = null!;

        /// <summary>
        /// De naam van de andere vereniging
        /// </summary>
        [DataMember(Name = "Naam")]
        public string Naam { get; set; } = null!;
    }
}
