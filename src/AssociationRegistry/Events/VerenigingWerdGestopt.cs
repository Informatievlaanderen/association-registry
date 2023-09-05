namespace AssociationRegistry.Events;

using Framework;

public record VerenigingWerdGestopt(DateOnly? Einddatum) : IEvent;
