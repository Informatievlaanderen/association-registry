namespace AssociationRegistry.Test.Projections.Framework;

using AssociationRegistry.Framework;

public record EventsPerVCode(string VCode, params IEvent[] Events);
