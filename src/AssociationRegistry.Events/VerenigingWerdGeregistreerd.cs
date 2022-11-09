namespace AssociationRegistry.Events;

/// <summary>
///
/// </summary>
/// <param name="VCode"></param>
/// <param name="Naam"></param>
/// <param name="KorteNaam"></param>
/// <param name="KorteBeschrijving"></param>
/// <param name="Startdatum">DateOnly, but marten doesnt seem to support it.</param>
/// <param name="KboNummer"></param>
/// <param name="Status"></param>
/// <param name="DatumLaatsteAanpassing">DateOnly, but marten doesnt seem to support it.</param>
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
