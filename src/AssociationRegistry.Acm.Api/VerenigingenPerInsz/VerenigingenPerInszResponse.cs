namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using System.Runtime.Serialization;

/// <summary>De lijst van alle verenigingen voor een specifiek INSZ</summary>
[DataContract]
public class VerenigingenPerInszResponse
{
    /// <summary>
    ///     Dit is de unieke identificatie van een persoon, dit kan een rijksregisternummer of bisnummer zijn
    /// </summary>
    [DataMember]
    public string Insz { get; init; } = null!;

    /// <summary>De lijst van verenigingen waarvoor deze persoon vertegenwoordiger is</summary>
    [DataMember]
    public Vereniging[] Verenigingen { get; init; } = null!;

    [DataContract]
    public class Vereniging
    {
        /// <summary>
        /// De vCode van de vereniging
        /// </summary>
        [DataMember]
        public string VCode { get; init; } = null!;

        /// <summary>
        /// De naam van de vereniging
        /// </summary>
        [DataMember]
        public int VertegenwoordigerId { get; init; }

        /// <summary>
        /// De naam van de vereniging
        /// </summary>
        [DataMember]
        public string Naam { get; init; } = null!;

        /// <summary>
        /// De status van de vereniging
        /// </summary>
        [DataMember]
        public string Status { get; init; } = null!;

        /// <summary>
        /// Het kbo nummer van de vereniging
        /// </summary>
        [DataMember]
        public string? KboNummer { get; init; }

        /// <summary>
        /// Is True als deze persoon een hoofdvertegenwoordiger is van deze vereniging
        /// </summary>
        [DataMember]
        public bool IsHoofdvertegenwoordigerVan { get; init; }
    }
}
