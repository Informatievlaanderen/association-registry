namespace AssociationRegistry.Test.Projections.Framework;

using ScenarioClassFixtures;

public interface IScenario
{
    public EventsPerVCode[] Events { get; }
}
