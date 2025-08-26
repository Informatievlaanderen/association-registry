﻿namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Alba;
using ApiSetup;
using DecentraalBeheer.Vereniging;
using JasperFx.Core;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Daemon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using Public.Api.Infrastructure.ConfigurationBindings;
using Scenarios.Requests;
using Vereniging;
using Xunit;

public abstract class TestContextBase<TScenario, TCommandRequest> : IDisposable, IAsyncLifetime
where TScenario : IScenario
{
    protected TestContextBase(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public FullBlownApiSetup ApiSetup { get; }
    public VCode VCode => CommandResult.VCode;
    protected abstract TScenario InitializeScenario();
    public CommandResult<TCommandRequest> CommandResult { get; protected set; }
    public TCommandRequest CommandRequest => CommandResult.Request;
    private IVCodeService VCodeService => ApiSetup.VCodeService;
    public TScenario Scenario { get; private set; }

    public AppSettings PublicApiAppSettings =>
        ApiSetup.PublicApiHost.Services.GetRequiredService<AppSettings>();
    public Hosts.Configuration.ConfigurationBindings.AppSettings AdminApiAppSettings =>
        ApiSetup.AdminApiHost.Services.GetRequiredService<Hosts.Configuration.ConfigurationBindings.AppSettings>();


    public async ValueTask InitializeAsync()
    {
        ApiSetup.Logger.LogWarning($"INITIALIZING Scenario: {Scenario}");

        Scenario = InitializeScenario();
        await using var lightweightSession = ApiSetup.AdminApiHost.DocumentStore().LightweightSession();
        var executedEvents = await ApiSetup.ExecuteGiven(Scenario, lightweightSession);

        ApiSetup.Logger.LogWarning($"WAITING FOR EVENTS TO BE PROCESSED: {Scenario}");

        if (executedEvents.Count != 0)
        {
            MaxSequenceByScenario = executedEvents.SelectMany(x => x.Value).Max(x => x.Sequence);

            await WaitBeforeCommands(ApiSetup, MaxSequenceByScenario.Value, ApiSetup.AdminProjectionHost, "BEFORE", lightweightSession);
            await WaitBeforeCommands(ApiSetup, MaxSequenceByScenario.Value, ApiSetup.PublicProjectionHost, "BEFORE", lightweightSession);
        }


        ApiSetup.Logger.LogWarning($"EXECUTING Scenario COMMAND REQUESTS: {Scenario}");
        await ExecuteScenario(Scenario);

        ApiSetup.Logger.LogWarning($"Command Result: {@CommandResult?.Sequence}");

        if (CommandResult.Sequence.HasValue)
        {
            await WaitBeforeCommands(ApiSetup, CommandResult.Sequence.Value, ApiSetup.AdminProjectionHost, "AFTER", lightweightSession);
            await WaitBeforeCommands(ApiSetup, CommandResult.Sequence.Value, ApiSetup.PublicProjectionHost, "AFTER", lightweightSession);
        }
        ApiSetup.Logger.LogWarning($"SCENARIO SETUP DONE: {Scenario}");
        await ApiSetup.ElasticClient.Indices.RefreshAsync(Indices.All);
    }

    public long? MaxSequenceByScenario { get; set; }

    private async Task WaitBeforeCommands(
        FullBlownApiSetup setup,
        long max,
        IAlbaHost host,
        string beforeOrAfter,
        IDocumentSession lightweightSession)
    {
        setup.Logger.LogWarning("{BEFOREORAFTER} COMMANDS: WAITING FOR REAL NOW", beforeOrAfter);

        var highWaterMark = max;

        var databaseSchemaName = host.Server.BaseAddress.ToString();

        setup.Logger.LogWarning("{BEFOREORAFTER} COMMANDS: Waiting for high water mark of {HighWaterMark} on database schema {DatabaseSchemaName}",
                          beforeOrAfter, highWaterMark, databaseSchemaName);

        var projectionProgress = await lightweightSession.DocumentStore.Advanced.AllProjectionProgress();

        setup.Logger.LogWarning("{BEFOREORAFTER} COMMANDS: Projection progress for {DatabaseSchemaName} is {ProjectionProgress}", beforeOrAfter, databaseSchemaName, projectionProgress);

        while (!projectionProgress.Any() || !projectionProgress.Any(x => !x.ShardName.Contains("ater")))
        {
            await Task.Delay(1.Seconds());
            projectionProgress = await lightweightSession.DocumentStore.Advanced.AllProjectionProgress();
            ApiSetup.Logger.LogInformation("WAITING FOR ANY projection progress");
        }

        var counter = 0;
        while (counter < 10
              && !projectionProgress.All(x => x.Sequence >= highWaterMark))
        {
            setup.Logger.LogWarning($"{beforeOrAfter} COMMANDS: WAITING FOR ANY projection progress for: " +
                                    $" {string.Join(", ", projectionProgress.Where(x => x.Sequence <= highWaterMark)
                                                                            .Select(x => $"{x.ShardName}: {x.Sequence}\n"))}");

            counter++;
            projectionProgress = await UpdateProjectionProgress(databaseSchemaName, setup, highWaterMark, lightweightSession);
        }

        if(!projectionProgress.All(x => x.Sequence >= highWaterMark))

        {
            setup.Logger.LogCritical("{BEFOREORAFTER} COMMANDS: Projection progress for {DatabaseSchemaName} is {ProjectionProgress}", beforeOrAfter, databaseSchemaName, projectionProgress);

            setup.Logger.LogCritical($"{beforeOrAfter} COMMANDS: Projection progress for {databaseSchemaName} was not high enough:" +
                                    string.Join(", ", projectionProgress.Where(x => x.Sequence <= highWaterMark)
                                                                        .Select(x => $"{x.ShardName}: {x.Sequence}\n")));
        }
        else
        {
            setup.Logger.LogCritical($"{beforeOrAfter} COMMANDS: Projection progress for {databaseSchemaName} was high enough!");
        }
    }

    private async Task<IReadOnlyList<ShardState>> UpdateProjectionProgress(string databaseSchemaName, FullBlownApiSetup setup, long? highWaterMark, IDocumentSession lightweightSession)
    {
        IReadOnlyList<ShardState> projectionProgress;
        setup.Logger.LogWarning("COMMANDS: Waiting for projection progress to be at least {HighWaterMark} on database schema {DatabaseSchemaName}",
                          highWaterMark, databaseSchemaName);
        await Task.Delay(2000);
        projectionProgress = await lightweightSession.DocumentStore.Advanced.AllProjectionProgress();
        setup.Logger.LogWarning("COMMANDS: Projection progress for {DatabaseSchemaName} is {ProjectionProgress}", databaseSchemaName, projectionProgress);

        return  projectionProgress;
    }



    protected abstract ValueTask ExecuteScenario(TScenario scenario);

    public void Dispose()
    {
    }

    public async ValueTask DisposeAsync()
        // Clear connection pool between tests to avoid leaked connections
        => Npgsql.NpgsqlConnection.ClearAllPools();
}
