namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Alba;
using ApiSetup;
using AssociationRegistry.Framework;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Microsoft.Extensions.Configuration;
using NodaTime;
using NodaTime.Text;
using Npgsql;

public abstract class End2EndContext<TRequest, TScenario> : IEnd2EndContext<TRequest> where TScenario : IScenario
{
    private static Mutex hostInitializationLock = new Mutex(false, nameof(hostInitializationLock));
    private readonly IApiSetup _fixture;
    public string VCode { get; protected set; }
    public abstract TRequest Request { get; }
    public abstract TScenario Scenario { get; }
    public IAlbaHost AdminApiHost { get; }
    public IAlbaHost QueryApiHost { get; }
    public IAlbaHost ProjectionHost { get; }

    protected End2EndContext(IApiSetup fixture)
    {
        _fixture = fixture;
        var schema = $"{GetType().Name[..^9]}{GetType().GetGenericArguments().First().Name.Substring(0, 3)}";

        _fixture.InitializeAsync(schema)
                .GetAwaiter().GetResult();

        AdminApiHost = _fixture.AdminApiHost;
        QueryApiHost = _fixture.QueryApiHost;
        ProjectionHost = _fixture.ProjectionHost;

        try
        {
            hostInitializationLock.WaitOne();
            AdminApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
            ProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
            QueryApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
        }
        finally
        {
            hostInitializationLock.ReleaseMutex();
        }
    }

    protected async Task Given(TScenario scenario)
    {
        await using (var session = AdminApiHost.DocumentStore().LightweightSession())
        {
            session.SetHeader(MetadataHeaderNames.Initiator, value: "metadata.Initiator");
            session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
            session.CorrelationId = Guid.NewGuid().ToString();

            session.Events.Append(scenario.VCode, scenario.GivenEvents(null));
            await session.SaveChangesAsync();
        }
    }

    protected async Task WaitForAdresMatchEvent()
    {
        await using var session = ProjectionHost.DocumentStore().LightweightSession();
        var events = await session.Events.FetchStreamAsync(Scenario.VCode);

        var counter = 0;

        while (!events.Any(a => a.EventType == typeof(AdresWerdOvergenomenUitAdressenregister)))
        {
            await Task.Delay(300);
            events = await session.Events.FetchStreamAsync(Scenario.VCode);

            if (++counter > 20)
                throw new Exception(
                    $"Kept waiting for Adresmatch... Events committed: {string.Join(separator: ", ", events.Select(x => x.EventTypeName))}");
        }
    }
}
