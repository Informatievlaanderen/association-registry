namespace AssociationRegistry.Events;

public record MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
    int LocatieId,
    string Naam,
    bool IsPrimair) : IEvent
{

}
