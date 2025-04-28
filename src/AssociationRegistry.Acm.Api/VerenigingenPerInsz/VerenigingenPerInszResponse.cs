namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using System.Runtime.Serialization;
using Vereniging;

/// <summary>De lijst van alle verenigingen voor een specifiek INSZ</summary>
[DataContract]
public class VerenigingenPerInszResponse
{
    /// <summary>
    ///     De unieke identificatie van de bevraagde persoon
    /// </summary>
    [DataMember]
    public string Insz { get; init; } = null!;

    /// <summary>De lijst van verenigingen waarvoor deze persoon vertegenwoordiger is</summary>
    [DataMember]
    public Vereniging[] Verenigingen { get; init; } = null!;

    /// <summary>De lijst van KBO-nummers waarvoor deze persoon vertegenwoordiger is</summary>
    [DataMember]
    public VerenigingenPerKbo[] KboNummers { get; init; } = null!;

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
        /// De id van de vertegenwoordiger
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
        /// Het KBO-nummer van de vereniging
        /// </summary>
        [DataMember]
        public string? KboNummer { get; init; }

        /// <summary>
        /// Het type van de vereniging
        /// </summary>
        [DataMember]
        public Verenigingstype Verenigingstype { get; set; } = null!;

        /// <summary>
        /// Het subtype van deze vereniging
        /// </summary>
        [DataMember(Name = "Verenigingssubtype", EmitDefaultValue = false)]
        public Verenigingssubtype? Verenigingssubtype { get; init; }

        /// <summary>
        /// Is True als deze persoon een hoofdvertegenwoordiger is van deze vereniging
        /// </summary>
        [DataMember]
        public bool IsHoofdvertegenwoordigerVan { get; init; }
    }

    /// <summary>
    /// Het subtype van de vereniging
    /// </summary>
    public class Verenigingssubtype : IVerenigingssubtypeCode
    {
        /// <summary>
        /// De code van het subtype vereniging
        /// </summary>
        [DataMember(Name = "Code")]
        public string Code { get; init; } = null!;

        /// <summary>
        /// De beschrijving van het subtype vereniging
        /// </summary>
        [DataMember(Name = "Naam")]
        public string Naam { get; init; } = null!;
    }


    public class VerenigingenPerKbo
    {
        /// <summary>
        /// Het KBO-nummer van de vereniging
        /// </summary>
        [DataMember]
        public string KboNummer { get; set; } = string.Empty;

        /// <summary>
        /// De vCode van de vereniging
        /// </summary>
        [DataMember]
        public string VCode { get; set; } = string.Empty;

        /// <summary>
        /// Is True als deze persoon een hoofdvertegenwoordiger is van deze vereniging
        /// </summary>
        [DataMember]
        public bool IsHoofdVertegenwoordiger { get; set; }
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
        public string Code { get; set; }

        /// <summary>
        /// De naam van het type van de vereniging
        /// </summary>
        [DataMember]
        public string Naam { get; set; }
    }
}


