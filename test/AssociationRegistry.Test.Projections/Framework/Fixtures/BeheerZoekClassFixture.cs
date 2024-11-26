namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using Admin.Schema.Search;

public class BeheerZoekClassFixture<TScenario> : ProjectionScenarioFixture<ProjectionContext, TScenario, VerenigingZoekDocument>
    where TScenario : IScenario, new()
{
    public BeheerZoekClassFixture(ProjectionContext context) : base(context, new TScenario())
    {
    }

    protected override async Task<VerenigingZoekDocument> FetchData()
        => (await Context
                 .AdminElasticClient
                 .GetAsync<VerenigingZoekDocument>(Scenario.VCode)).Source;
}
