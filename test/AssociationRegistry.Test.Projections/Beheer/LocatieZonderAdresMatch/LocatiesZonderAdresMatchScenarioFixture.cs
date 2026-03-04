namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Projections.Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;

public class LocatiesZonderAdresMatchScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, LocatieZonderAdresMatchDocument[], ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon) =>
        await daemon.RebuildProjectionAsync<LocatieZonderAdresMatchProjection>(CancellationToken.None);

    protected override async Task<LocatieZonderAdresMatchDocument[]> GetResultAsync(
        IDocumentSession session,
        TScenario scenario
    ) =>
        (
            await session
                .Query<LocatieZonderAdresMatchDocument>()
                .Where(x => x.VCode == scenario.AggregateId)
                .ToListAsync()
        ).ToArray();
}

public class LocatiesZonderAdresMatchScenarioClassFixture<TScenario>
    : IClassFixture<LocatiesZonderAdresMatchScenarioFixture<TScenario>>
    where TScenario : IScenario, new() { }
