namespace AssociationRegistry.Test.Projections.Framework;

using AutoFixture;
using Common.AutoFixture;

public interface IScenario
{
    public string AggregateId { get; }
    public EventsPerVCode[] Events { get; }
}

public interface IInszScenario : IScenario
{
    public string Insz { get; }
}

public abstract class InszScenarioBase : IInszScenario
{
    public Fixture AutoFixture { get; }

    public InszScenarioBase()
    {
        AutoFixture = new Fixture().CustomizeDomain();
    }

    public abstract string AggregateId { get; }
    public abstract string Insz { get; }
    public abstract EventsPerVCode[] Events { get; }
}
