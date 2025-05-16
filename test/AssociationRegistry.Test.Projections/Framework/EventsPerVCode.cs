namespace AssociationRegistry.Test.Projections.Framework;

using AssociationRegistry.Framework;
using Events;

public record EventsPerVCode(string VCode, params IEvent[] Events);
