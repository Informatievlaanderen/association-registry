namespace AssociationRegistry.Events;

public record SubverenigingDetailsWerdenGewijzigd(string VCode, string Identificatie, string Beschrijving) : IEvent
{ }
