namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

/// <summary>
///     Is de authentieke vereniging een vereniging met rechtspersoonlijkheid, dan worden de gegevens die enkel via de
///     KBO-sync bijgewerkt worden niet overgenomen: de korte naam, de startdatum en de vertegenwoordigers.
///     De gegevens die zo'n vereniging zelf mag beheren, worden zoals bij elke andere vereniging samengevoegd.
/// </summary>
public static class VerenigingMetRechtspersoonlijkheidSamenvoeging
{
    public const string RedenBeheerdViaKbo =
        "De authentieke vereniging is een vereniging met rechtspersoonlijkheid; dit gegeven wordt enkel via de KBO bijgewerkt.";

    public static bool IsVerenigingMetRechtspersoonlijkheid(VerenigingState vereniging) =>
        Verenigingstype.IsKboVereniging(vereniging.Verenigingstype.Code);
}
