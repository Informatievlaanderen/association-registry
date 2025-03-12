namespace AssociationRegistry.Events;

public record DetailGegevensVanDeSubverenigingRelatieWerdenGewijzigd(string VCode, string Identificatie, string Beschrijving) : IEvent
{ }
