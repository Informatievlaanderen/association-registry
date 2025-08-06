namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Relatie
{
    /// <summary>
    ///     Het type relatie
    /// </summary>
    [DataMember(Name = "Relatietype")]
    public string Relatietype { get; set; } = null!;

    /// <summary>
    ///     de gerelateerde vereniging
    /// </summary>
    [DataMember(Name = "AndereVereniging")]
    public GerelateerdeVereniging AndereVereniging { get; set; } = null!;

    [DataContract]
    public class GerelateerdeVereniging
    {
        /// <summary>
        ///     Het KBO nummer van de gerelateerde vereniging
        /// </summary>
        [DataMember(Name = "KboNummer")]
        public string KboNummer { get; set; } = null!;

        /// <summary>
        ///     De unieke identificator van de gerelateerde vereniging in het verenigingsregister
        /// </summary>
        [DataMember(Name = "VCode")]
        public string VCode { get; set; } = null!;

        /// <summary>
        ///     De naam van de gerelateerde vereniging
        /// </summary>
        [DataMember(Name = "Naam")]
        public string Naam { get; set; } = null!;

        /// <summary>
        ///     De link naar het publiek detail van de gerelateerde vereniging
        /// </summary>
        [DataMember(Name = "Detail")]
        public string Detail { get; set; } = string.Empty;
    }
}
