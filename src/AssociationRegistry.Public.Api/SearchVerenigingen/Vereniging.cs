using System.Collections.Immutable;

namespace AssociationRegistry.Public.Api.SearchVerenigingen;

public record Vereniging(
    string Id,
    string Naam,
    string KorteNaam,
    ImmutableArray<Locatie> Locaties,
    ImmutableArray<Activiteit> Activiteiten);

public record Locatie(string Type, string AdresId, string Adresvoorstelling);

public record Activiteit(int Id, string Categorie);
