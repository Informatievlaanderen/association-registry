namespace AssociationRegistry.Acties.WijzigBasisgegevens;

using Vereniging;

public record WijzigBasisgegevensCommand(
    VCode VCode,
    VerenigingsNaam? Naam = null,
    string? KorteNaam = null,
    string? KorteBeschrijving = null,
    Startdatum? Startdatum = default,
    Doelgroep? Doelgroep = null,
    HoofdactiviteitVerenigingsloket[]? HoofdactiviteitenVerenigingsloket = null,
    bool? IsUitgeschrevenUitPubliekeDatastroom = null);
