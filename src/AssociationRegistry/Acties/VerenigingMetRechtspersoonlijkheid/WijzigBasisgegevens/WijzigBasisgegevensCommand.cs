namespace AssociationRegistry.Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;

using Vereniging;

public record WijzigBasisgegevensCommand(
    VCode VCode,
    string? Roepnaam = null,
    string? KorteBeschrijving = null,
    HoofdactiviteitVerenigingsloket[]? HoofdactiviteitenVerenigingsloket = null);
