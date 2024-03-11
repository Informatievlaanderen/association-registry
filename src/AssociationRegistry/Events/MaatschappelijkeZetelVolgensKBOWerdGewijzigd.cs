namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
    int LocatieId,
    string Naam,
    bool IsPrimair) : IEvent
{
    public static MaatschappelijkeZetelVolgensKBOWerdGewijzigd With(Locatie locatie)
        => new(locatie.LocatieId, locatie.Naam ?? string.Empty, locatie.IsPrimair);
}
