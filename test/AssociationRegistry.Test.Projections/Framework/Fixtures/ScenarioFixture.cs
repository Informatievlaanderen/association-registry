namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

public abstract class ScenarioFixture<TScenario, TResult, TContext>(TContext context) : IAsyncLifetime
    where TScenario : IScenario, new()
    where TContext : IProjectionContext
{
    public TContext Context { get; } = context;
    public TScenario Scenario { get; } = new();
    public TResult Result { get; private set; }

    public async Task InitializeAsync()
    {
        await Context.SaveAsync(Scenario.Events);
        await Context.RefreshDataAsync();

        Result = await GetResultAsync(Scenario);
    }

    protected abstract Task<TResult> GetResultAsync(TScenario scenario);
    public Task DisposeAsync() => Task.CompletedTask;
}
