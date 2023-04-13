namespace AssociationRegistry.DuplicateVerenigingDetection;

using System.Collections.Immutable;

public record DuplicaatVereniging(
    string VCode,
    string Naam,
    string KorteNaam,
    ImmutableArray<DuplicaatVereniging.HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket,
    string Doelgroep,
    ImmutableArray<DuplicaatVereniging.Locatie> Locaties,
    ImmutableArray<DuplicaatVereniging.Activiteit> Activiteiten)
{
    public record Locatie(
        string Locatietype,
        bool Hoofdlocatie,
        string Adres,
        string? Naam,
        string Postcode,
        string Gemeente);

    public record Activiteit(int Id, string Categorie);

    public record HoofdactiviteitVerenigingsloket(string Code, string Beschrijving);
}
