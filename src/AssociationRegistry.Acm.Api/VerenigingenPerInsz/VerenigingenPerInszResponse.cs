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
        /// Array of strings - vCodes die dezelfde vereniging weergeven (na correctie van dubbele registraties)
        /// </summary>
        [DataMember]
        public string[] CorresponderendeVCodes { get; set; } = null!;

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

        // <summary>
        /// Het kbo nummer van de vereniging
        /// </summary>
        [DataMember]
        public Verenigingstype Verenigingstype { get; set; } = null!;

        /// <summary>
        /// Is True als deze persoon een hoofdvertegenwoordiger is van deze vereniging
        /// </summary>
        [DataMember]
        public bool IsHoofdvertegenwoordigerVan { get; init; }
    }
}

[DataContract]
public class Verenigingstype
{
    public Verenigingstype(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    /// <summary>
    /// De code van het type van de vereniging
    /// </summary>
    [DataMember]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De naam van het type van de vereniging
    /// </summary>
    [DataMember]
    public string Naam { get; set; } = null!;
}
