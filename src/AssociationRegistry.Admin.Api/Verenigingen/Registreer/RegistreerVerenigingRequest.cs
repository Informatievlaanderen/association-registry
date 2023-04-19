namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Acties.RegistreerVereniging;
using Common;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;

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
    public ToeTeVoegenContactgegeven[] Contactgegevens { get; set; } = Array.Empty<ToeTeVoegenContactgegeven>();

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember]
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();

    /// <summary>De vertegenwoordigers van deze vereniging</summary>
    [DataMember]
    public Vertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<Vertegenwoordiger>();

    /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
    [DataMember]
    public string[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<string>();

    public RegistreerVerenigingCommand ToCommand()
        => new(
            VerenigingsNaam.Create(Naam),
            KorteNaam,
            KorteBeschrijving,
            AssociationRegistry.Vereniging.Startdatum.Create(Startdatum),
            AssociationRegistry.Vereniging.KboNummer.Create(KboNummer),
            Contactgegevens.Select(Map).ToArray(),
            Locaties.Select(Map).ToArray(),
            Vertegenwoordigers.Select(Map).ToArray(),
            HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.Create).ToArray());

    private static AssociationRegistry.Vereniging.Vertegenwoordiger Map(Vertegenwoordiger vert)
        => AssociationRegistry.Vereniging.Vertegenwoordiger.Create(
            Insz.Create(vert.Insz!),
            vert.PrimairContactpersoon,
            vert.Roepnaam,
            vert.Rol,
            Email.Create(vert.Email),
            TelefoonNummer.Create(vert.Telefoon),
            TelefoonNummer.Create(vert.Mobiel),
            SocialMedia.Create(vert.SocialMedia)
        );

    private static AssociationRegistry.Vereniging.Locatie Map(Locatie loc)
        => AssociationRegistry.Vereniging.Locatie.Create(
            loc.Naam,
            loc.Straatnaam,
            loc.Huisnummer,
            loc.Busnummer,
            loc.Postcode,
            loc.Gemeente,
            loc.Land,
            loc.Hoofdlocatie,
            loc.Locatietype);

    public static Contactgegeven Map(ToeTeVoegenContactgegeven toeTeVoegenContactgegeven)
        => Contactgegeven.Create(
            ContactgegevenType.Parse(toeTeVoegenContactgegeven.Type),
            toeTeVoegenContactgegeven.Waarde,
            toeTeVoegenContactgegeven.Beschrijving,
            toeTeVoegenContactgegeven.IsPrimair);

    /// <summary>Een vertegenwoordiger van een vereniging</summary>
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

        /// <summary>Het emailadres van de vertegenwoordiger</summary>
        [DataMember]
        public string? Email { get; set; }

        /// <summary>Het telefoonnummer van de vertegenwoordiger</summary>
        [DataMember]
        public string? Telefoon { get; set; }

        /// <summary>Het mobiel nummer van de vertegenwoordiger</summary>
        [DataMember]
        public string? Mobiel { get; set; }

        /// <summary>Het socialmedia account van de vertegenwoordiger</summary>
        [DataMember]
        public string? SocialMedia { get; set; }
    }

    /// <summary>Een locatie van een vereniging</summary>
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
