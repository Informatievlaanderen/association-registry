namespace AssociationRegistry.Vereniging.WijzigBasisgegevens;

using Primitives;
using Startdatums;

public record WijzigBasisgegevensCommand(
    string VCode,
    string? Naam = null,
    string? KorteNaam = null,
    string? KorteBeschrijving = null,
    Startdatum? Startdatum = default);
