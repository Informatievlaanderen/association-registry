namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using AssociationRegistry.Framework;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Events.Daemon;
using NodaTime;
using NodaTime.Text;

public abstract class ScenarioFixture<TScenario, TResult, TContext>(TContext context) : IAsyncLifetime
    where TScenario : IScenario, new()
    where TContext : IProjectionContext
{
    public TContext Context { get; } = context;
    public TScenario Scenario { get; } = new();
    public TResult Result { get; private set; }

    public string MetadataInitiator = "metadata.Initiator";
    public readonly string MetadataTijdstip = InstantPattern.General.Format(new Instant());

    public async ValueTask InitializeAsync()
    {
        var store = Context.AdminStore;
        await store.Advanced.Clean.DeleteAllDocumentsAsync();
        await store.Advanced.Clean.DeleteAllEventDataAsync();
        var session = store.LightweightSession();
        session.SetHeader(MetadataHeaderNames.Initiator, MetadataInitiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, MetadataTijdstip);
        session.CorrelationId = Guid.NewGuid().ToString();

        await Context.SaveAsync(Scenario.Events, session);

        var currentStoreSession = DocumentStore.LightweightSession();
        using var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await RefreshProjectionsAsync(daemon);
        Result = await GetResultAsync(currentStoreSession, Scenario);
    }

    protected abstract IDocumentStore DocumentStore { get; }
    protected abstract Task RefreshProjectionsAsync(IProjectionDaemon daemon);
    protected abstract Task<TResult> GetResultAsync(
        IDocumentSession session,
        TScenario scenario);
    public ValueTask DisposeAsync() => new(Task.CompletedTask);
}
