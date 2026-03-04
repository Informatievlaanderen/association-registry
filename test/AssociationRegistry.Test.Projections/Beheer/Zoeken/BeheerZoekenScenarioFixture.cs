namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using System;
using Admin.ProjectionHost.Projections;
using Admin.ProjectionHost.Projections.Search.Zoeken;
using Admin.Schema.Search;
using Elastic.Clients.Elasticsearch;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;
using Polly;

public class BeheerZoekenScenarioFixture<TScenario>
    : ScenarioFixture<TScenario, VerenigingZoekDocument, ProjectionContext>
    where TScenario : IScenario, new()
{
    private readonly ElasticsearchDocumentRetriever<VerenigingZoekDocument> _documentRetriever;
    private readonly ProjectionRefresher _projectionRefresher;

    public BeheerZoekenScenarioFixture(ProjectionContext context)
        : base(context)
    {
        _documentRetriever = new ElasticsearchDocumentRetriever<VerenigingZoekDocument>(context.AdminElasticClient);
        _projectionRefresher = new ProjectionRefresher(context.AdminElasticClient);
    }

    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(BeheerZoekProjectionHandler.ShardName.Name, CancellationToken.None);
        await _projectionRefresher.RefreshAsync();
    }

    protected override async Task<VerenigingZoekDocument> GetResultAsync(IDocumentSession _, TScenario scenario)
    {
        return await _documentRetriever.GetDocumentAsync(scenario.AggregateId);
    }
}

public class BeheerZoekenScenarioClassFixture<TScenario> : IClassFixture<BeheerZoekenScenarioFixture<TScenario>>
    where TScenario : IScenario, new() { }
