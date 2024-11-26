namespace AssociationRegistry.Test.Projections.Framework;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;

public abstract class ScenarioBase : IScenario
{
    public Fixture AutoFixture { get; }

    public ScenarioBase()
    {
        AutoFixture = new Fixture().CustomizeDomain();
    }

    public abstract EventsPerVCode[] Events { get; }
}
