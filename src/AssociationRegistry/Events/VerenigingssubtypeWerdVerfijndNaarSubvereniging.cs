namespace AssociationRegistry.Events;

public record VerenigingssubtypeWerdVerfijndNaarSubvereniging(string VCode, Registratiedata.SubverenigingVan SubverenigingVan) : IEvent
{ }
