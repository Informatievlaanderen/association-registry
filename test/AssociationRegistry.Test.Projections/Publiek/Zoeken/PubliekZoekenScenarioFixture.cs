﻿namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;
using Public.Schema.Search;

public class PubliekZoekenScenarioFixture<TScenario>(ProjectionContext context)
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

public class PubliekZoekenScenarioClassFixture<TScenario>
    : IClassFixture<PubliekZoekenScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
