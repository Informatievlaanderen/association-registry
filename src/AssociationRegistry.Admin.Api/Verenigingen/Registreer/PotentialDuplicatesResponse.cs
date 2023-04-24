namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using Acties.RegistreerVereniging;
using DuplicateVerenigingDetection;
using Infrastructure.ConfigurationBindings;

[DataContract]
public class PotentialDuplicatesResponse
{
    public PotentialDuplicatesResponse(string hashedRequest, PotentialDuplicatesFound potentialDuplicates, AppSettings appSettings)
    {
        BevestigingsToken = hashedRequest;
        MogelijkeDuplicateVerenigingen = potentialDuplicates.Candidates.Select(c => FromDuplicaatVereniging(c, appSettings)).ToArray();
    }

    /// <summary>Dit token wordt gebruikt als bevestiging dat de vereniging uniek is en geregistreerd mag worden,
    /// ondanks de voorgestelde duplicaten.</summary>
    [DataMember(Name = "BevestigingsToken")]
    public string BevestigingsToken { get; }

    /// <summary>Een gelimiteerde lijst van bestaande verenigingen die mogelijks een duplicaat
    /// zouden kunnen zijn van de te registreren vereniging</summary>
    [DataMember(Name = "MogelijkeDuplicateVerenigingen")]
    public DuplicaatVerenigingContract[] MogelijkeDuplicateVerenigingen { get; }

    private static DuplicaatVerenigingContract FromDuplicaatVereniging(DuplicaatVereniging document, AppSettings appSettings)
        => new(
            document.VCode,
            document.Naam,
            document.KorteNaam,
            document.HoofdactiviteitenVerenigingsloket.Select(DuplicaatVerenigingContract.HoofdactiviteitVerenigingsloket.FromDuplicaatVereniging).ToImmutableArray(),
            document.Doelgroep,
            document.Locaties.Select(DuplicaatVerenigingContract.Locatie.FromDuplicaatVereniging).ToImmutableArray(),
            document.Activiteiten.Select(DuplicaatVerenigingContract.Activiteit.FromDuplicaatVereniging).ToImmutableArray(),
            new DuplicaatVerenigingContract.VerenigingLinks(new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{(string?)document.VCode}")));

    /// <summary>Een mogelijke duplicaat van de te registreren vereniging</summary>
    [DataContract]
    public class DuplicaatVerenigingContract
    {
        public DuplicaatVerenigingContract(string vCode,
            string naam,
            string korteNaam,
            ImmutableArray<HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket,
            string doelgroep,
            ImmutableArray<Locatie> locaties,
            ImmutableArray<Activiteit> activiteiten,
            VerenigingLinks links)
        {
            VCode = vCode;
            Naam = naam;
            KorteNaam = korteNaam;
            HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloket;
            Doelgroep = doelgroep;
            Locaties = locaties;
            Activiteiten = activiteiten;
            Links = links;
        }

        /// <summary>De unieke identificatie code van deze vereniging</summary>
        [DataMember(Name = "VCode")]
        public string VCode { get; init; }

        /// <summary>Naam van de vereniging</summary>
        [DataMember(Name = "Naam")]
        public string Naam { get; init; }

        /// <summary>Korte naam van de vereniging</summary>
        [DataMember(Name = "KorteNaam")]
        public string KorteNaam { get; init; }

        /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
        [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
        public ImmutableArray<HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket { get; init; }

        /// <summary>De doelgroep waarop de vereniging zich richt</summary>
        [DataMember(Name = "Doelgroep")]
        public string Doelgroep { get; init; }

        /// <summary>Alle locaties waar deze vereniging actief is</summary>
        [DataMember(Name = "Locaties")]
        public ImmutableArray<Locatie> Locaties { get; init; }

        /// <summary>De activiteiten die de vereniging uitvoert</summary>
        [DataMember(Name = "Activiteiten")]
        public ImmutableArray<Activiteit> Activiteiten { get; init; }

        /// <summary>Weblinks i.v.m. deze vereniging</summary>
        [DataMember(Name = "Links")]
        public VerenigingLinks Links { get; init; }

        [DataContract]
        public class Locatie
        {
            public Locatie(string locatietype,
                bool hoofdlocatie,
                string adres,
                string? naam,
                string postcode,
                string gemeente)
            {
                Locatietype = locatietype;
                Hoofdlocatie = hoofdlocatie;
                Adres = adres;
                Naam = naam;
                Postcode = postcode;
                Gemeente = gemeente;
            }

            public static Locatie FromDuplicaatVereniging(DuplicaatVereniging.Locatie locatie)
                => new(locatie.Locatietype, locatie.Hoofdlocatie, locatie.Adres, locatie.Naam, locatie.Postcode, locatie.Gemeente);

            [DataMember(Name = "Locatietype")]
            public string Locatietype { get; init; }

            [DataMember(Name = "Hoofdlocatie", EmitDefaultValue = false)]
            public bool Hoofdlocatie { get; init; }

            [DataMember(Name = "Adres")] public string Adres { get; init; }
            [DataMember(Name = "Naam")] public string? Naam { get; init; }

            [DataMember(Name = "Postcode")]
            public string Postcode { get; init; }

            [DataMember(Name = "Gemeente")]
            public string Gemeente { get; init; }
        }

        [DataContract]
        public class Activiteit
        {
            public Activiteit(int id,
                string categorie)
            {
                Id = id;
                Categorie = categorie;
            }

            public static Activiteit FromDuplicaatVereniging(DuplicaatVereniging.Activiteit locatie)
                => new(locatie.Id, locatie.Categorie);

            [DataMember(Name = "Id")] public int Id { get; init; }

            [DataMember(Name = "Categorie")]
            public string Categorie { get; init; }
        }

        [DataContract]
        public class HoofdactiviteitVerenigingsloket
        {
            public HoofdactiviteitVerenigingsloket(string code,
                string beschrijving)
            {
                Code = code;
                Beschrijving = beschrijving;
            }

            public static HoofdactiviteitVerenigingsloket FromDuplicaatVereniging(DuplicaatVereniging.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
                => new(hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Beschrijving);

            [DataMember(Name = "Code")] public string Code { get; init; }

            [DataMember(Name = "Beschrijving")]
            public string Beschrijving { get; init; }
        }

        /// <summary>Weblinks i.v.m. deze vereniging</summary>
        [DataContract]
        public class VerenigingLinks
        {
            public VerenigingLinks(Uri detail)
            {
                Detail = detail;
            }

            [DataMember(Name = "Detail")]
            public Uri Detail { get; init; }
        }
    }
}
