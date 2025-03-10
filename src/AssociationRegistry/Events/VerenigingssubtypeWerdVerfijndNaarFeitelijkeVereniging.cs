namespace AssociationRegistry.Events;

public record VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(string VCode, Registratiedata.Subtype Subtype) : IEvent
{ }
