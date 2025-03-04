namespace AssociationRegistry.Events;

public record SubtypeWerdTerugGezetNaarNogNietBepaald(string VCode, Registratiedata.Subtype Subtype) : IEvent
{ }
