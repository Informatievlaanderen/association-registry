namespace AssociationRegistry.Test.Projections.Framework;

using Events;

public record EventsPerVCode(string VCode, params IEvent[] Events);
