namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using System.Collections.Immutable;
using System.Runtime.Serialization;

[DataContract]
public record DetailVerenigingResponse(
    [property: DataMember(Name = "Vereniging")]
    DetailVerenigingResponse.VerenigingDetail Vereniging,
    [property: DataMember(Name = "Metadata")]
    DetailVerenigingResponse.MetadataDetail Metadata)
{
    [DataContract]
    public class VerenigingDetail
    {
        public VerenigingDetail(string vCode,
            string naam,
            string? korteNaam,
            string? korteBeschrijving,
            string? startdatum,
            string? kboNummer,
            string status,
            ImmutableArray<Contactgegeven> contactgegevens,
            ImmutableArray<Locatie> locaties,
            ImmutableArray<Vertegenwoordiger> vertegenwoordigers,
            ImmutableArray<HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket)
        {
            VCode = vCode;
            Naam = naam;
            KorteNaam = korteNaam;
            KorteBeschrijving = korteBeschrijving;
            Startdatum = startdatum;
            KboNummer = kboNummer;
            Status = status;
            Contactgegevens = contactgegevens;
            Locaties = locaties;
            Vertegenwoordigers = vertegenwoordigers;
            HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloket;
        }

        /// <summary>De unieke identificatie code van deze vereniging</summary>
        [DataMember(Name = "VCode")]
        public string VCode { get; init; }

        /// <summary>Naam van de vereniging</summary>
        [DataMember(Name = "Naam")]
        public string Naam { get; init; }

        /// <summary>Korte naam van de vereniging</summary>
        [DataMember(Name = "KorteNaam")]
        public string? KorteNaam { get; init; }

        /// <summary>Korte beschrijving van de vereniging</summary>
        [DataMember(Name = "KorteBeschrijving")]
        public string? KorteBeschrijving { get; init; }

        /// <summary>Datum waarop de vereniging gestart is</summary>
        [DataMember(Name = "Startdatum")]
        public string? Startdatum { get; init; }

        /// <summary>
        ///     Ondernemingsnummer van de vereniging. Formaat '##########', '#### ### ###' en '####.###.###" zijn toegelaten
        /// </summary>
        [DataMember(Name = "KboNummer")]
        public string? KboNummer { get; init; }

        /// <summary>Status van de vereniging</summary>
        [DataMember(Name = "Status")]
        public string Status { get; init; }

        /// <summary>De contactgegevens van deze vereniging</summary>
        [DataMember(Name = "Contactgegevens")]
        public ImmutableArray<Contactgegeven> Contactgegevens { get; init; }

        /// <summary>Alle locaties waar deze vereniging actief is</summary>
        [DataMember(Name = "Locaties")]
        public ImmutableArray<Locatie> Locaties { get; init; }

        /// <summary>Alle vertegenwoordigers van deze vereniging</summary>
        [DataMember(Name = "Vertegenwoordigers")]
        public ImmutableArray<Vertegenwoordiger> Vertegenwoordigers { get; init; }

        /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
        [DataMember(Name = "hoofdactiviteitenVerenigingsloket")]
        public ImmutableArray<HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket { get; init; }

        /// <summary>Een contactgegeven van een vereniging</summary>
        [DataContract]
        public class Contactgegeven
        {
            public Contactgegeven(int contactgegevenId,
                string type,
                string waarde,
                string? beschrijving,
                bool isPrimair)
            {
                ContactgegevenId = contactgegevenId;
                Type = type;
                Waarde = waarde;
                Beschrijving = beschrijving;
                IsPrimair = isPrimair;
            }

            /// <summary>De unieke identificatie code van dit contactgegevens binnen de vereniging</summary>
            [DataMember(Name = "ContactgegevenId")]
            public int ContactgegevenId { get; init; }

            /// <summary>Het type contactgegeven</summary>
            [DataMember(Name = "Type")]
            public string Type { get; init; }

            /// <summary>De waarde van het contactgegeven</summary>
            [DataMember(Name = "Waarde")]
            public string Waarde { get; init; }

            /// <summary>
            /// Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
            /// </summary>
            [DataMember(Name = "Beschrijving")]
            public string? Beschrijving { get; init; }

            /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
            [DataMember(Name = "IsPrimair")]
            public bool IsPrimair { get; init; }
        }

        /// <summary>Een vertegenwoordiger van een vereniging</summary>
        [DataContract]
        public class Vertegenwoordiger
        {
            public Vertegenwoordiger(string insz,
                string voornaam,
                string achternaam,
                string? roepnaam,
                string? rol,
                bool primairContactpersoon,
                string email,
                string telefoon,
                string mobiel,
                string socialMedia)
            {
                Insz = insz;
                Voornaam = voornaam;
                Achternaam = achternaam;
                Roepnaam = roepnaam;
                Rol = rol;
                PrimairContactpersoon = primairContactpersoon;
                Email = email;
                Telefoon = telefoon;
                Mobiel = mobiel;
                SocialMedia = socialMedia;
            }

            /// <summary>
            ///     Dit is de unieke identificatie van een vertegenwoordiger, dit kan een rijksregisternummer of bisnummer zijn
            /// </summary>
            [DataMember(Name = "Insz")]
            public string Insz { get; init; }

            /// <summary>Dit is de voornaam van de vertegenwoordiger volgens het rijksregister</summary>
            [DataMember(Name = "Voornaam")]
            public string Voornaam { get; init; }

            /// <summary>Dit is de achternaam van de vertegenwoordiger volgens het rijksregister</summary>
            [DataMember(Name = "Achternaam")]
            public string Achternaam { get; init; }

            /// <summary>Dit is de roepnaam van de vertegenwoordiger</summary>
            [DataMember(Name = "Roepnaam")]
            public string? Roepnaam { get; init; }

            /// <summary>Dit is de rol van de vertegenwoordiger binnen de vereniging</summary>
            [DataMember(Name = "Rol")]
            public string? Rol { get; init; }

            /// <summary>
            ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
            /// </summary>
            [DataMember(Name = "PrimairContactpersoon")]
            public bool PrimairContactpersoon { get; init; }

            /// <summary>Het emailadres van de vertegenwoordiger</summary>
            [DataMember(Name = "Email")]
            public string Email { get; init; }

            /// <summary>Het telefoonnummer van de vertegenwoordiger</summary>
            [DataMember(Name = "Telefoon")]
            public string Telefoon { get; init; }

            /// <summary>Het mobiel nummer van de vertegenwoordiger</summary>
            [DataMember(Name = "Mobiel")]
            public string Mobiel { get; init; }

            /// <summary>Het socialmedia account van de vertegenwoordiger</summary>
            [DataMember(Name = "SocialMedia")]
            public string SocialMedia { get; init; }
        }

        /// <summary>Een locatie van een vereniging</summary>
        [DataContract]
        public class Locatie
        {
            public Locatie(string locatietype,
                bool hoofdlocatie,
                string adres,
                string? naam,
                string straatnaam,
                string huisnummer,
                string? busnummer,
                string postcode,
                string gemeente,
                string land)
            {
                Locatietype = locatietype;
                Hoofdlocatie = hoofdlocatie;
                Adres = adres;
                Naam = naam;
                Straatnaam = straatnaam;
                Huisnummer = huisnummer;
                Busnummer = busnummer;
                Postcode = postcode;
                Gemeente = gemeente;
                Land = land;
            }

            /// <summary>
            ///     Het soort locatie dat beschreven word<br />
            ///     <br />
            ///     Mogelijke waarden:<br />
            ///     - Activiteiten<br />
            ///     - Correspondentie - Slecht één maal mogelijk<br />
            /// </summary>
            [DataMember(Name = "Locatietype")]
            public string Locatietype { get; init; }

            /// <summary>Duidt aan dat dit de uniek hoofdlocatie is</summary>
            [DataMember(Name = "Hoofdlocatie", EmitDefaultValue = false)]
            public bool Hoofdlocatie { get; init; }

            /// <summary>Een standaard geformatteerde weergave van het adres van de locatie</summary>
            [DataMember(Name = "Adres")]
            public string Adres { get; init; }

            /// <summary>Een beschrijvende naam voor de locatie</summary>
            [DataMember(Name = "Naam")]
            public string? Naam { get; init; }

            /// <summary>De straat van de locatie</summary>
            [DataMember(Name = "Straatnaam")]
            public string Straatnaam { get; init; }

            /// <summary>Het huisnummer van de locatie</summary>
            [DataMember(Name = "Huisnummer")]
            public string Huisnummer { get; init; }

            /// <summary>Het busnummer van de locatie</summary>
            [DataMember(Name = "Busnummer")]
            public string? Busnummer { get; init; }

            /// <summary>De postcode van de locatie</summary>
            [DataMember(Name = "Postcode")]
            public string Postcode { get; init; }

            /// <summary>De gemeente van de locatie</summary>
            [DataMember(Name = "Gemeente")]
            public string Gemeente { get; init; }

            /// <summary>Het land van de locatie</summary>
            [DataMember(Name = "Land")]
            public string Land { get; init; }
        }

        /// <summary>De hoofdactivititeit van een vereniging volgens het verenigingsloket</summary>
        [DataContract]
        public class HoofdactiviteitVerenigingsloket
        {
            public HoofdactiviteitVerenigingsloket(string code,
                string beschrijving)
            {
                Code = code;
                Beschrijving = beschrijving;
            }

            /// <summary>De code van de hoofdactivititeit</summary>
            [DataMember(Name = "Code")]
            public string Code { get; init; }

            /// <summary>De beschrijving van de hoofdactivititeit</summary>
            [DataMember(Name = "Beschrijving")]
            public string Beschrijving { get; init; }
        }
    }

    /// <summary>De metadata van de vereniging, deze bevat bv de datum van laatste aanpassing</summary>
    public class MetadataDetail
    {
        /// <summary>De datum waarop de laatste aanpassing uitgevoerd is op de gegevens van de vereniging</summary>
        /// <param name="datumLaatsteAanpassing"></param>
        public MetadataDetail(string datumLaatsteAanpassing)
        {
            DatumLaatsteAanpassing = datumLaatsteAanpassing;
        }

        public string DatumLaatsteAanpassing { get; init; }
    }
}
