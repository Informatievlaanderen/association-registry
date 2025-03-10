namespace AssociationRegistry.Events;

public record SubtypeWerdVerfijndNaarFeitelijkeVereniging(string VCode, Registratiedata.Subtype Subtype) : IEvent
{ }
