namespace AssociationRegistry.Events;


using Vereniging;

public record MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
    int LocatieId,
    string Naam,
    bool IsPrimair) : IEvent
{

}
