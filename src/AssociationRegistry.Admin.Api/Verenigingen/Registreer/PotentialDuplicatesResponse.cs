namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using Infrastructure.ConfigurationBindings;
using Vereniging.DuplicateDetection;
using Vereniging.RegistreerVereniging;

public class PotentialDuplicatesResponse
{
    public string BevestigingsToken { get; }
    public DuplicaatVerenigingContract[] MogelijkeDuplicateVerenigingen { get; }

    public PotentialDuplicatesResponse(string hashedRequest, PotentialDuplicatesFound potentialDuplicates, AppSettings appSettings)
    {
        BevestigingsToken = hashedRequest;
        MogelijkeDuplicateVerenigingen = potentialDuplicates.Candidates.Select(c=>FromDuplicaatVereniging(c,appSettings)).ToArray();
    }

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

    [DataContract]
    public record DuplicaatVerenigingContract(
        [property: DataMember(Name = "VCode")] string VCode,
        [property: DataMember(Name = "Naam")] string Naam,
        [property: DataMember(Name = "KorteNaam")]
        string KorteNaam,
        [property: DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
        ImmutableArray<DuplicaatVerenigingContract.HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket,
        [property: DataMember(Name = "Doelgroep")]
        string Doelgroep,
        [property: DataMember(Name = "Locaties")]
        ImmutableArray<DuplicaatVerenigingContract.Locatie> Locaties,
        [property: DataMember(Name = "Activiteiten")]
        ImmutableArray<DuplicaatVerenigingContract.Activiteit> Activiteiten,
        [property: DataMember(Name = "Links")] DuplicaatVerenigingContract.VerenigingLinks Links)
    {
        [DataContract]
        public record Locatie(
            [property: DataMember(Name = "Locatietype")]
            string Locatietype,
            [property: DataMember(Name = "Hoofdlocatie", EmitDefaultValue = false)]
            bool Hoofdlocatie,
            [property: DataMember(Name = "Adres")] string Adres,
            [property: DataMember(Name = "Naam")] string? Naam,
            [property: DataMember(Name = "Postcode")]
            string Postcode,
            [property: DataMember(Name = "Gemeente")]
            string Gemeente
        )
        {
            public static Locatie FromDuplicaatVereniging(DuplicaatVereniging.Locatie locatie)
                => new(locatie.Locatietype, locatie.Hoofdlocatie, locatie.Adres, locatie.Naam, locatie.Postcode, locatie.Gemeente);
        }

        [DataContract]
        public record Activiteit(
            [property: DataMember(Name = "Id")] int Id,
            [property: DataMember(Name = "Categorie")]
            string Categorie)
        {
            public static Activiteit FromDuplicaatVereniging(DuplicaatVereniging.Activiteit locatie)
                => new(locatie.Id, locatie.Categorie);
        }

        [DataContract]
        public record HoofdactiviteitVerenigingsloket(
            [property: DataMember(Name = "Code")] string Code,
            [property: DataMember(Name = "Beschrijving")]
            string Beschrijving)
        {
            public static HoofdactiviteitVerenigingsloket FromDuplicaatVereniging(DuplicaatVereniging.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
                => new(hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Beschrijving);
        }

        [DataContract]
        public record VerenigingLinks(
            [property: DataMember(Name = "Detail")]
            Uri Detail
        );
    }
}
