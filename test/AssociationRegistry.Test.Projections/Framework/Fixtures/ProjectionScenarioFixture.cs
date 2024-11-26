using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Projections.Framework;
using AutoFixture;
using Xunit;

public record EventsPerVCode(string VCode, params IEvent[] Events);

public interface IScenario
{
    string VCode { get; }
    IEnumerable<EventsPerVCode> GivenEvents { get; }
}

public abstract class ProjectionScenarioFixture<TProjectionContext, TScenario, TDocument> : IDisposable, IAsyncLifetime
    where TProjectionContext : IProjectionContext
    where TScenario : IScenario
{
    public TScenario Scenario { get; }
    public TProjectionContext Context { get; }
    public Fixture AutoFixture { get; }

    protected ProjectionScenarioFixture(TProjectionContext context, TScenario scenario)
    {
        Scenario = scenario;
        Context = context;
        AutoFixture = new Fixture().CustomizeDomain();
    }

    public void Dispose()
    {
    }

    public async Task InitializeAsync()
    {
        await Context.SaveEvents(Scenario.GivenEvents);
        Document = await FetchData();
    }

    protected abstract Task<TDocument> FetchData();
    public TDocument Document { get; set; }

    public async Task DisposeAsync()
    {
    }
}
