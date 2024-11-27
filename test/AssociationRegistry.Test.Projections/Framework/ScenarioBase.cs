namespace AssociationRegistry.Test.Projections.Framework;

using AutoFixture;
using Common.AutoFixture;

public abstract class ScenarioBase : IScenario
{
    public Fixture AutoFixture { get; }

    public ScenarioBase()
    {
        AutoFixture = new Fixture().CustomizeDomain();
    }

    public abstract string VCode { get; }
    public abstract EventsPerVCode[] Events { get; }
}
