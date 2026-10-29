namespace AssociationRegistry.EventStore;

using Events;

/// <summary>De nieuwe events voor één aggregate, met de versie waarop het aggregate geladen werd.</summary>
public record AggregateEvents(string AggregateId, long? AggregateVersion, IEvent[] Events);
