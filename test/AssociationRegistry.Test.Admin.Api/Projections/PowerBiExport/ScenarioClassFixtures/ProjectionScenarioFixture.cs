namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport.ScenarioClassFixtures;

using AutoFixture;
using Marten;
using Test.Framework.Customizations;
using Xunit;

public abstract class ProjectionScenarioFixture: IDisposable, IAsyncLifetime
{
    public ProjectionContext Context { get; }
    public Fixture AutoFixture { get; }

    public abstract Task Given();

    protected ProjectionScenarioFixture(ProjectionContext context)
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
