namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using System.Runtime.Serialization;
using Projections.Detail;

[DataContract]
public class DetailVerenigingResponse
{
    [DataMember(Name = "Vereniging")] public VerenigingDetail Vereniging { get; init; } = null!;

    [DataMember(Name = "Metadata")] public MetadataDetail Metadata { get; init; } = null!;

    [DataContract]
    public class VerenigingDetail
    {
        /// <summary>De unieke identificatie code van deze vereniging</summary>
        [DataMember(Name = "VCode")]
        public string VCode { get; init; } = null!;

        /// <summary>Het type vereniging</summary>
        [DataMember(Name = "Type")]
        public BeheerVerenigingDetailDocument.VerenigingsType Type { get; init; } = null!;

        /// <summary>Naam van de vereniging</summary>
        [DataMember(Name = "Naam")]
        public string Naam { get; init; } = null!;

        /// <summary>Korte naam van de vereniging</summary>
        [DataMember(Name = "KorteNaam")]
        public string? KorteNaam { get; init; }

        /// <summary>Korte beschrijving van de vereniging</summary>
        [DataMember(Name = "KorteBeschrijving")]
        public string? KorteBeschrijving { get; init; }

        /// <summary>Datum waarop de vereniging gestart is</summary>
        [DataMember(Name = "Startdatum")]
        public string? Startdatum { get; init; }

        /// <summary>Status van de vereniging</summary>
        [DataMember(Name = "Status")]
        public string Status { get; init; } = null!;

        /// <summary>De contactgegevens van deze vereniging</summary>
        [DataMember(Name = "Contactgegevens")]
        public Contactgegeven[] Contactgegevens { get; init; } = null!;

        /// <summary>Alle locaties waar deze vereniging actief is</summary>
        [DataMember(Name = "Locaties")]
        public Locatie[] Locaties { get; init; } = null!;

        /// <summary>Alle vertegenwoordigers van deze vereniging</summary>
        [DataMember(Name = "Vertegenwoordigers")]
        public Vertegenwoordiger[] Vertegenwoordigers { get; init; } = null!;

        /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
        [DataMember(Name = "hoofdactiviteitenVerenigingsloket")]
        public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = null!;

        /// <summary>De sleutels van deze vereniging beheren</summary>
        [DataMember(Name = "Sleutels")]
        public Sleutel[] Sleutels { get; init; } = null!;


        /// <summary>Een contactgegeven van een vereniging</summary>
        [DataContract]
        public class Contactgegeven
        {
            /// <summary>De unieke identificatie code van dit contactgegeven binnen de vereniging</summary>
            [DataMember(Name = "ContactgegevenId")]
            public int ContactgegevenId { get; init; }

            /// <summary>Het type contactgegeven</summary>
            [DataMember(Name = "Type")]
            public string Type { get; init; } = null!;

            /// <summary>De waarde van het contactgegeven</summary>
            [DataMember(Name = "Waarde")]
            public string Waarde { get; init; } = null!;

            /// <summary>
            ///     Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
            /// </summary>
            [DataMember(Name = "Beschrijving")]
            public string? Beschrijving { get; init; }

            /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
            [DataMember(Name = "IsPrimair")]
            public bool IsPrimair { get; init; }
        }

        [DataContract]
        public class Sleutel
        {
            /// <summary>
            /// De bron van de sleutel
            /// </summary>
            [DataMember(Name = "Bron")]
            public string Bron { get; set; } = null!;

            /// <summary>
            /// De externe identificator van de vereniging in de bron
            /// </summary>
            [DataMember(Name = "Waarde")]
            public string Waarde { get; set; } = null!;
        }

        /// <summary>Een vertegenwoordiger van een vereniging</summary>
        [DataContract]
        public class Vertegenwoordiger
        {
            /// <summary>
            ///     De unieke identificatie code van deze vertegenwoordiger binnen de vereniging
            /// </summary>
            [DataMember(Name = "VertegenwoordigerId")]
            public int VertegenwoordigerId { get; set; }

            /// <summary>Dit is de voornaam van de vertegenwoordiger volgens het rijksregister</summary>
            [DataMember(Name = "Voornaam")]
            public string Voornaam { get; init; } = null!;

            /// <summary>Dit is de achternaam van de vertegenwoordiger volgens het rijksregister</summary>
            [DataMember(Name = "Achternaam")]
            public string Achternaam { get; init; } = null!;

            /// <summary>Dit is de roepnaam van de vertegenwoordiger</summary>
            [DataMember(Name = "Roepnaam")]
            public string? Roepnaam { get; init; }

            /// <summary>Dit is de rol van de vertegenwoordiger binnen de vereniging</summary>
            [DataMember(Name = "Rol")]
            public string? Rol { get; init; }

            /// <summary>
            ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
            /// </summary>
            [DataMember(Name = "IsPrimair")]
            public bool PrimairContactpersoon { get; init; }

            /// <summary>Het emailadres van de vertegenwoordiger</summary>
            [DataMember(Name = "Email")]
            public string Email { get; init; } = null!;

            /// <summary>Het telefoonnummer van de vertegenwoordiger</summary>
            [DataMember(Name = "Telefoon")]
            public string Telefoon { get; init; } = null!;

            /// <summary>Het mobiel nummer van de vertegenwoordiger</summary>
            [DataMember(Name = "Mobiel")]
            public string Mobiel { get; init; } = null!;

            /// <summary>Het socialmedia account van de vertegenwoordiger</summary>
            [DataMember(Name = "SocialMedia")]
            public string SocialMedia { get; init; } = null!;
        }

        /// <summary>Een locatie van een vereniging</summary>
        [DataContract]
        public class Locatie
        {
            /// <summary>
            ///     Het soort locatie dat beschreven wordt<br />
            ///     <br />
            ///     Mogelijke waarden:<br />
            ///     - Activiteiten<br />
            ///     - Correspondentie - Slecht één maal mogelijk<br />
            /// </summary>
            [DataMember(Name = "Locatietype")]
            public string Locatietype { get; init; } = null!;

            /// <summary>Duidt aan dat dit de hoofdlocatie is</summary>
            [DataMember(Name = "Hoofdlocatie")]
            public bool Hoofdlocatie { get; init; }

            /// <summary>Een standaard geformatteerde weergave van het adres van de locatie</summary>
            [DataMember(Name = "Adres")]
            public string Adres { get; init; } = null!;

            /// <summary>Een beschrijvende naam voor de locatie</summary>
            [DataMember(Name = "Naam")]
            public string? Naam { get; init; }

            /// <summary>De straat van de locatie</summary>
            [DataMember(Name = "Straatnaam")]
            public string Straatnaam { get; init; } = null!;

            /// <summary>Het huisnummer van de locatie</summary>
            [DataMember(Name = "Huisnummer")]
            public string Huisnummer { get; init; } = null!;

            /// <summary>Het busnummer van de locatie</summary>
            [DataMember(Name = "Busnummer")]
            public string? Busnummer { get; init; }

            /// <summary>De postcode van de locatie</summary>
            [DataMember(Name = "Postcode")]
            public string Postcode { get; init; } = null!;

            /// <summary>De gemeente van de locatie</summary>
            [DataMember(Name = "Gemeente")]
            public string Gemeente { get; init; } = null!;

            /// <summary>Het land van de locatie</summary>
            [DataMember(Name = "Land")]
            public string Land { get; init; } = null!;
        }

        /// <summary>De hoofdactivititeit van een vereniging volgens het verenigingsloket</summary>
        [DataContract]
        public class HoofdactiviteitVerenigingsloket
        {
            /// <summary>De code van de hoofdactivititeit</summary>
            [DataMember(Name = "Code")]
            public string Code { get; init; } = null!;

            /// <summary>De beschrijving van de hoofdactivititeit</summary>
            [DataMember(Name = "Beschrijving")]
            public string Beschrijving { get; init; } = null!;
        }
    }

    /// <summary>De metadata van de vereniging, deze bevat bv de datum van laatste aanpassing</summary>
    public class MetadataDetail
    {
        /// <summary>De datum waarop de laatste aanpassing uitgevoerd is op de gegevens van de vereniging</summary>
        public string DatumLaatsteAanpassing { get; init; } = null!;

        public string BeheerBasisUri { get; init; } = null!;
    }
}
