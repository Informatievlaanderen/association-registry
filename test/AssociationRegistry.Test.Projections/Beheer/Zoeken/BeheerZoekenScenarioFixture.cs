namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema.Search;
using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;

public class BeheerZoekenScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, VerenigingZoekDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<VerenigingZoekDocument> GetResultAsync(
        TScenario scenario,
        IDocumentSession session,
        IProjectionDaemon daemon)
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
