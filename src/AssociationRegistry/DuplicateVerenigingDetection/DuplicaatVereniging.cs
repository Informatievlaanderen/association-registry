namespace AssociationRegistry.DuplicateVerenigingDetection;

using System.Collections.Immutable;
using Vereniging;


public record DuplicaatVereniging(
    string VCode,
    DuplicaatVereniging.VerenigingsType Verenigingstype,
    string Naam,
    string KorteNaam,
    ImmutableArray<DuplicaatVereniging.HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket,
    ImmutableArray<DuplicaatVereniging.Locatie> Locaties,
    DuplicaatVereniging.ScoringInfo? Scoring = null
    )
{
    public record Locatie(
        string Locatietype,
        bool IsPrimair,
        string Adres,
        string? Naam,
        string Postcode,
        string Gemeente);

    public record VerenigingsType : IVerenigingstype
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
