namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using AutoFixture;
using Common.AutoFixture;

public abstract class ProjectionScenarioFixture<TProjectionContext> : IDisposable, IAsyncLifetime
    where TProjectionContext : IProjectionContext
{
    public TProjectionContext Context { get; }
    public Fixture AutoFixture { get; }
    public abstract Task Given();

    protected ProjectionScenarioFixture(TProjectionContext context)
    {
        Context = context;
        AutoFixture = new Fixture().CustomizeDomain();
    }

    public void Dispose()
    {
    }

    public async ValueTask  InitializeAsync()
    {
        await Given();
    }

    public async ValueTask DisposeAsync()
    {
    }
}
