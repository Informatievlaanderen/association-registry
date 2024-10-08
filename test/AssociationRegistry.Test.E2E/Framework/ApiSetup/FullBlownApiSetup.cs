﻿namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Admin.Api;
using Alba;
using AlbaHost;
using AssociationRegistry.Framework;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.RuntimeCompiler.Scenarios;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using Oakton;
using TestClasses;
using Vereniging;
using Xunit;
using ProjectionHostProgram = Public.ProjectionHost.Program;

public class FullBlownApiSetup : IAsyncLifetime, IApiSetup
{

    public FullBlownApiSetup()
    {
    }

    public string? AuthCookie { get; private set; }
    public ILogger<Program> Logger { get; private set; }
    public IAlbaHost AdminApiHost { get; private set; }
    public IAlbaHost AdminProjectionHost { get; private set; }
    public IAlbaHost PublicProjectionHost { get; private set; }
    public IAlbaHost PublicApiHost { get; private set; }

    public async Task InitializeAsync()
    {
        var schema = "fullblowne2e";
        OaktonEnvironment.AutoStartHost = true;

        AdminApiHost = (await AlbaHost.For<Program>(ConfigureForTesting(schema, "adminapi")))
           .EnsureEachCallIsAuthenticated();

        AdminApiHost.EnsureEachCallIsAuthenticated();

        AdminProjectionHost = await AlbaHost.For<Admin.ProjectionHost.Program>(
            ConfigureForTesting(schema, "adminproj"));
        Logger = AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

        PublicProjectionHost = await AlbaHost.For<ProjectionHostProgram>(
            ConfigureForTesting(schema, "publicproj"));

        PublicApiHost = await AlbaHost.For<Public.Api.Program>(
            ConfigureForTesting(schema, "publicapi"));


        await AdminApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await AdminProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await PublicProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await PublicApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await PublicProjectionHost.ResumeAllDaemonsAsync();
        await AdminProjectionHost.ResumeAllDaemonsAsync();
    }

    private Action<IWebHostBuilder> ConfigureForTesting(string schema, string name)
    {
        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.development.json", false)
                           .AddJsonFile("appsettings.e2e.json", false)
                           .AddJsonFile($"appsettings.e2e.{name}.json", false)
                           .Build();

        return b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());

            b.UseConfiguration(configuration);

            b.ConfigureServices((context, services) =>
              {
                  context.HostingEnvironment.EnvironmentName = "Development";
                  services.Configure<PostgreSqlOptionsSection>(s => { s.Schema = schema; });
              })
             .UseSetting(key: "ASPNETCORE_ENVIRONMENT", value: "Development")
             .UseSetting(key: "ApplyAllDatabaseChangesDisabled", value: "true");
        };
    }

    public async Task DisposeAsync()
    {
        await AdminApiHost.StopAsync();
        await PublicApiHost.StopAsync();
        await AdminProjectionHost.StopAsync();
        await PublicProjectionHost.StopAsync();
        await AdminApiHost.DisposeAsync();
        await PublicApiHost.DisposeAsync();
        await PublicProjectionHost.DisposeAsync();
        await AdminProjectionHost.DisposeAsync();
    }

    public async Task ExecuteGiven(IScenario emptyScenario)
    {
        var documentStore = AdminApiHost.DocumentStore();

        await using var session = documentStore.LightweightSession();
        session.SetHeader(MetadataHeaderNames.Initiator, value: "metadata.Initiator");
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
        session.CorrelationId = Guid.NewGuid().ToString();

        foreach (var eventsPerStream in await emptyScenario.GivenEvents(AdminApiHost.Services.GetRequiredService<IVCodeService>()))
        {
            session.Events.Append(eventsPerStream.Key, eventsPerStream.Value);
        }

        await session.SaveChangesAsync();

        await AdminProjectionHost.DocumentStore().WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(30));
        await PublicProjectionHost.DocumentStore().WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(30));
    }
}
