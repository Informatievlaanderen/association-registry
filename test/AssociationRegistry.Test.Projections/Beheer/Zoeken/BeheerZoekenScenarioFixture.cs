namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema.Search;
using Framework.Fixtures;

public class BeheerZoekenScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, VerenigingZoekDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<VerenigingZoekDocument> GetResultAsync(
        TScenario scenario)
    {
      var getResponse =
            await Context.ElasticClient
                         .GetAsync<VerenigingZoekDocument>(scenario.VCode);

        return getResponse.Source;
    }
}

public class BeheerZoekenScenarioClassFixture<TScenario>
    : IClassFixture<BeheerZoekenScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
