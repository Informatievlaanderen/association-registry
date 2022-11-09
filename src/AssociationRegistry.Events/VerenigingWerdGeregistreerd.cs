namespace AssociationRegistry.Events;

/// <summary>
///
/// </summary>
/// <param name="VCode"></param>
/// <param name="Naam"></param>
/// <param name="KorteNaam"></param>
/// <param name="KorteBeschrijving"></param>
/// <param name="Startdatum"></param>
/// <param name="KboNummer"></param>
/// <param name="Status"></param>
/// <param name="DatumLaatsteAanpassing"></param>
public record VerenigingWerdGeregistreerd(
    string VCode,
    string Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    DateOnly? Startdatum,
    string? KboNummer,
    string Status,
    DateOnly DatumLaatsteAanpassing
) : IEvent;
