namespace AssociationRegistry.DecentraalBeheer.Vereniging.DuplicaatDetectie;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using System.Collections.Immutable;

public record DuplicaatVereniging(
    string VCode,
    DuplicaatVereniging.Types.Verenigingstype Verenigingstype,
    DuplicaatVereniging.Types.Verenigingssubtype? Verenigingssubtype,
    string Naam,
    string KorteNaam,
    ImmutableArray<DuplicaatVereniging.Types.HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket,
    ImmutableArray<DuplicaatVereniging.Types.Locatie> Locaties,
    DuplicaatVereniging.Types.ScoringInfo? Scoring = null
    )
{
    public static class Types
    {
        public record Locatie(
            string Locatietype,
            bool IsPrimair,
            string Adres,
            string? Naam,
            string Postcode,
            string Gemeente);

        public record Verenigingstype : IVerenigingstype
        {
            public string Code { get; init; }
            public string Naam { get; init; }
        }

        public record Verenigingssubtype : IVerenigingssubtypeCode
        {
            public string Code { get; init; }
            public string Naam { get; init; }
        }

        public record Activiteit(int Id, string Categorie);
        public record HoofdactiviteitVerenigingsloket(string Code, string Naam);

        public record ScoringInfo(string Explanation, double? Score)
        {
            public static ScoringInfo NotApplicable => new("N/A", null);
        };
    }
}
