namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

public abstract class ScenarioFixture<TScenario, TResult, TContext> : IAsyncLifetime
    where TScenario : IScenario, new()
    where TContext : IProjectionContext
{
    public TContext Context { get; }
    public TScenario Scenario { get; } = new();
    public TResult Result { get; private set; }

    protected ScenarioFixture(TContext context)
    {
        Context = context;
    }

    public async Task InitializeAsync()
    {
        await Context.SaveAsync(Scenario.Events);

        Result = await GetResultAsync(Scenario);
    }

    public Task DisposeAsync() => Task.CompletedTask;
    public abstract Task<TResult> GetResultAsync(TScenario scenario);
}
