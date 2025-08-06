namespace AssociationRegistry.DecentraalBeheer.Acties.Basisgegevens.VerenigingMetRechtspersoonlijkheid;

using Vereniging;

public record WijzigBasisgegevensCommand(
    VCode VCode,
    string? Roepnaam = null,
    string? KorteBeschrijving = null,
    Doelgroep? Doelgroep = null,
    HoofdactiviteitVerenigingsloket[]? HoofdactiviteitenVerenigingsloket = null,
    Werkingsgebied[]? Werkingsgebieden = null);
