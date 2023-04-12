namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using ContactGegevens;
using Infrastructure.Swagger;
using Primitives;
using Vereniging.RegistreerVereniging;

[DataContract]
public class RegistreerVerenigingRequest
{
    /// <summary>Instantie die de vereniging aanmaakt</summary>
    [DataMember]
    [Required]
    public string Initiator { get; init; } = null!;

    /// <summary>Naam van de vereniging</summary>
    [DataMember]
    [Required]
    public string Naam { get; init; } = null!;

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de vereniging gestart is. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember]
    public DateOnly? Startdatum { get; init; }

    /// <summary>
    ///     Ondernemingsnummer van de vereniging. Formaat '##########', '#### ### ###' en '####.###.###" zijn toegelaten
    /// </summary>
    [DataMember]
    public string? KboNummer { get; init; }

    /// <summary>De contactgegevens van deze vereniging</summary>
    [DataMember]
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember]
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();

    /// <summary>De vertegenwoordigers van deze vereniging</summary>
    [DataMember]
    public Vertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<Vertegenwoordiger>();

    /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
    [DataMember]
    public string[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<string>();

    public RegistreerVerenigingCommand ToRegistreerVerenigingCommand()
        => new(
            Naam,
            KorteNaam,
            KorteBeschrijving,
            Startdatums.Startdatum.Create(Startdatum),
            KboNummer,
            Contactgegevens.Select(Contactgegeven.ToCommand).ToArray(),
            Locaties.Select(ToLocatie).ToArray(),
            Vertegenwoordigers.Select(ToVertegenwoordiger).ToArray(),
            HoofdactiviteitenVerenigingsloket);

    private static RegistreerVerenigingCommand.Vertegenwoordiger ToVertegenwoordiger(Vertegenwoordiger vert)
        => new(
            vert.Insz!,
            vert.PrimairContactpersoon,
            vert.Roepnaam,
            vert.Rol,
            vert.Contactgegevens.Select(Contactgegeven.ToCommand).ToArray());

    private static RegistreerVerenigingCommand.Locatie ToLocatie(Locatie loc)
        => new(
            loc.Naam,
            loc.Straatnaam,
            loc.Huisnummer,
            loc.Busnummer,
            loc.Postcode,
            loc.Gemeente,
            loc.Land,
            loc.Hoofdlocatie,
            loc.Locatietype);

    /// <summary>
    /// Het toe te voegen contactgegeven
    /// </summary>
    [DataContract]
    public class Contactgegeven
    {
        /// <summary>
        /// Het type contactgegeven.
        /// </summary>
        [SwaggerParameterExample("Email")]
        [SwaggerParameterExample("SocialMedia")]
        [SwaggerParameterExample("Telefoon")]
        [SwaggerParameterExample("Website")]
        [DataMember(Name = "type")] public string Type { get; set; } = null!;

        /// <summary>
        /// De waarde van het contactgegeven
        /// </summary>
        [DataMember(Name = "waarde")] public string Waarde { get; set; } = null!;

        /// <summary>
        /// Vrij veld die het het contactgegeven omschrijft (bijv: algemeen, administratie, ...)
        /// </summary>
        [DataMember(Name = "omschrijving")] public string? Omschrijving { get; set; }

        /// <summary>
        /// Duidt het contactgegeven aan als primair contactgegeven
        /// </summary>
        [DataMember(Name = "isPrimair", EmitDefaultValue = false)]
        public bool IsPrimair { get; set; }

        public static RegistreerVerenigingCommand.Contactgegeven ToCommand(Contactgegeven contactgegeven)
            => new(
                ContactgegevenType.Parse(contactgegeven.Type),
                contactgegeven.Waarde,
                contactgegeven.Omschrijving,
                contactgegeven.IsPrimair);
    }

    [DataContract]
    public class Vertegenwoordiger
    {
        /// <summary>
        ///     Dit is de unieke identificatie van een vertegenwoordiger, dit kan een rijksregisternummer of bisnummer zijn
        /// </summary>
        [DataMember]
        public string? Insz { get; set; }

        /// <summary>Dit is de rol van de vertegenwoordiger binnen de vereniging</summary>
        [DataMember]
        public string? Rol { get; set; }

        /// <summary>Dit is de roepnaam van de vertegenwoordiger</summary>
        [DataMember]
        public string? Roepnaam { get; set; }

        /// <summary>
        ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
        /// </summary>
        [DataMember]
        public bool PrimairContactpersoon { get; set; }

        /// <summary>Dit zijn de contactgegevens van een vertegenwoordiger</summary>
        [DataMember]
        public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    }

    [DataContract]
    public class Locatie
    {
        /// <summary>
        ///     Het soort locatie dat beschreven word<br />
        ///     <br />
        ///     Mogelijke waarden:<br />
        ///     - Activiteiten<br />
        ///     - Correspondentie - Slecht één maal mogelijk<br />
        /// </summary>
        [DataMember]
        public string Locatietype { get; set; } = null!;

        /// <summary>Duidt aan dat dit de uniek hoofdlocatie is</summary>
        [DataMember]
        public bool Hoofdlocatie { get; set; }

        /// <summary>Een beschrijvende naam voor de locatie</summary>
        [DataMember]
        public string? Naam { get; set; }

        /// <summary>De straat van de locatie</summary>
        [DataMember]
        public string Straatnaam { get; set; } = null!;

        /// <summary>Het huisnummer van de locatie</summary>
        [DataMember]
        public string Huisnummer { get; set; } = null!;

        /// <summary>Het busnummer van de locatie</summary>
        [DataMember]
        public string? Busnummer { get; set; }

        /// <summary>De postcode van de locatie</summary>
        [DataMember]
        public string Postcode { get; set; } = null!;

        /// <summary>De gemeente van de locatie</summary>
        [DataMember]
        public string Gemeente { get; set; } = null!;

        /// <summary>Het land van de locatie</summary>
        [DataMember]
        public string Land { get; set; } = null!;
    }
}
