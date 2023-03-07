namespace AssociationRegistry.Vereniging.WijzigBasisgegevens;

using Primitives;

public record WijzigBasisgegevensCommand(
    string VCode,
    string? Naam = null,
    string? KorteNaam = null,
    string? KorteBeschrijving = null,
    NullOrEmpty<DateOnly> Startdatum = default);
