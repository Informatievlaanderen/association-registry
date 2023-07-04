namespace AssociationRegistry.DuplicateVerenigingDetection;

using System.Collections.Immutable;

public record DuplicaatVereniging(
    string VCode,
    DuplicaatVereniging.VerenigingsType Type,
    string Naam,
    string KorteNaam,
    ImmutableArray<DuplicaatVereniging.HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket,
    ImmutableArray<DuplicaatVereniging.Locatie> Locaties)
{
    public record Locatie(
        string Locatietype,
        bool IsPrimair,
        string Adres,
        string? Naam,
        string Postcode,
        string Gemeente);

    public record VerenigingsType(string Code, string Beschrijving);


    public record Activiteit(int Id, string Categorie);

    public record HoofdactiviteitVerenigingsloket(string Code, string Beschrijving);
}
