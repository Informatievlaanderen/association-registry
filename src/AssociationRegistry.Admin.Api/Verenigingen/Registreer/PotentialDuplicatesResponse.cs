namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
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

    /// <summary>Een lijst van verenigingen die mogelijks een duplicaat zijn
    /// van de vereniging uit de registreer aanvraag</summary>
    [DataMember(Name = "MogelijkeDuplicateVerenigingen")]
    public DuplicaatVerenigingContract[] MogelijkeDuplicateVerenigingen { get; }

    private static DuplicaatVerenigingContract FromDuplicaatVereniging(DuplicaatVereniging document, AppSettings appSettings)
        => new(
            document.VCode,
            document.Naam,
            document.KorteNaam,
            new DuplicaatVerenigingContract.VerenigingsType(document.Type.Code, document.Type.Beschrijving),
            document.HoofdactiviteitenVerenigingsloket.Select(DuplicaatVerenigingContract.HoofdactiviteitVerenigingsloket.FromDuplicaatVereniging).ToImmutableArray(),
            document.Locaties.Select(DuplicaatVerenigingContract.Locatie.FromDuplicaatVereniging).ToImmutableArray(),
            new DuplicaatVerenigingContract.VerenigingLinks(new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{(string?)document.VCode}")));

    /// <summary>Een mogelijke duplicaat van de te registreren vereniging</summary>
    [DataContract]
    public class DuplicaatVerenigingContract
    {
        public DuplicaatVerenigingContract(string vCode,
            string naam,
            string korteNaam,
            VerenigingsType type,
            ImmutableArray<HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket,
            ImmutableArray<Locatie> locaties,
            VerenigingLinks links)
        {
            VCode = vCode;
            Naam = naam;
            KorteNaam = korteNaam;
            Type = type;
            HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloket;
            Locaties = locaties;
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

        /// <summary>Type van de vereniging</summary>
        [DataMember(Name = "Type")]
        public VerenigingsType Type { get; init; }

        /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
        [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
        public ImmutableArray<HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket { get; init; }

        /// <summary>Alle locaties waar deze vereniging actief is</summary>
        [DataMember(Name = "Locaties")]
        public ImmutableArray<Locatie> Locaties { get; init; }

        /// <summary>Weblinks i.v.m. deze vereniging</summary>
        [DataMember(Name = "Links")]
        public VerenigingLinks Links { get; init; }

        /// <summary>Een locatie van een vereniging</summary>
        [DataContract]
        public class Locatie
        {
            public Locatie(string locatietype,
                bool isPrimair,
                string adres,
                string? naam,
                string postcode,
                string gemeente)
            {
                Locatietype = locatietype;
                IsPrimair = isPrimair;
                Adres = adres;
                Naam = naam;
                Postcode = postcode;
                Gemeente = gemeente;
            }

            public static Locatie FromDuplicaatVereniging(DuplicaatVereniging.Locatie locatie)
                => new(locatie.Locatietype, locatie.IsPrimair, locatie.Adres, locatie.Naam, locatie.Postcode, locatie.Gemeente);

            /// <summary>Het soort locatie dat beschreven wordt</summary>
            [DataMember(Name = "Locatietype")]
            public string Locatietype { get; init; }

            /// <summary>Duidt aan dat dit de primaire locatie is</summary>
            [DataMember(Name = "IsPrimair")]
            public bool IsPrimair { get; init; }

            /// <summary>Het samengestelde adres van de locatie</summary>
            [DataMember(Name = "Adresvoorstelling")]
            public string Adres { get; init; }

            /// <summary>Een beschrijvende naam voor de locatie</summary>
            [DataMember(Name = "Naam")]
            public string? Naam { get; init; }

            /// <summary>Het busnummer van de locatie</summary>
            [DataMember(Name = "Postcode")]
            public string Postcode { get; init; }

            /// <summary>De gemeente van de locatie</summary>
            [DataMember(Name = "Gemeente")]
            public string Gemeente { get; init; }
        }

        [DataContract]
        public class VerenigingsType
        {

            public VerenigingsType(string code,
                string beschrijving)
            {
                Code = code;
                Beschrijving = beschrijving;
            }

            public static VerenigingsType FromDuplicaatVereniging(DuplicaatVereniging duplicaatVereniging)
                => new(duplicaatVereniging.Type.Code, duplicaatVereniging.Type.Beschrijving);

            /// <summary>De code van het type van deze vereniging</summary>
            [DataMember(Name = "Code")] public string Code { get; }

            /// <summary>De beschrijving van het type van deze vereniging</summary>
            [DataMember(Name = "Beschrijving")]public string Beschrijving { get; }
        }

        /// <summary>Een activiteit van een vereninging</summary>
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

            /// <summary>De unieke identificatie code van deze activiteit binnen de vereniging</summary>
            [DataMember(Name = "Id")] public int Id { get; init; }

            /// <summary>De categorie van deze activiteit</summary>
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

            /// <summary>De code van de hoofdactivititeit</summary>
            [DataMember(Name = "Code")]
            public string Code { get; init; }

            /// <summary>De beschrijving van de hoofdactivititeit</summary>
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

            /// <summary>De link naar het beheer detail van de vereniging</summary>
            [DataMember(Name = "Detail")]
            public Uri Detail { get; init; }
        }
    }
}
