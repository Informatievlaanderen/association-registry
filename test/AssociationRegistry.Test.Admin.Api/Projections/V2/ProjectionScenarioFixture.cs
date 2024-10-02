namespace AssociationRegistry.Test.Admin.Api.Projections.V2;

using AssociationRegistry.Test.Framework.Customizations;
using AutoFixture;
using Xunit;

public abstract class ProjectionScenarioFixture<TProjectionContext>: IDisposable, IAsyncLifetime
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

    public async Task InitializeAsync()
    {
        await Given();
    }

    public async Task DisposeAsync()
    {
    }
}
