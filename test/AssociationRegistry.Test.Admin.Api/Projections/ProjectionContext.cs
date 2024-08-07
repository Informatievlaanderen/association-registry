namespace AssociationRegistry.Test.Admin.Api.Projections;

using Alba;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oakton;
using Xunit;
using ProjectionHostProgram = AssociationRegistry.Admin.ProjectionHost.Program;

[CollectionDefinition(nameof(ProjectionContext))]
public class RegistreerVerenigingCollection : ICollectionFixture<ProjectionContext>
{ }

public class ProjectionContext : IAsyncLifetime
{
    public string? AuthCookie { get; private set; }
    public ILogger<ProjectionHostProgram> Logger { get; private set; }
    public IAlbaHost ProjectionHost { get; private set; }

    public async Task InitializeAsync()
    {
        OaktonEnvironment.AutoStartHost = true;

        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.json").Build();

        ProjectionHost = await AlbaHost.For<ProjectionHostProgram>(ConfigureForTesting(configuration, "projecties"));

        await ProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await ProjectionHost.ResumeAllDaemonsAsync();
    }

    public async Task DisposeAsync()
        => throw new NotImplementedException();

    private Action<IWebHostBuilder> ConfigureForTesting(IConfigurationRoot configuration, string schema)
    {
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
             .UseSetting(key: $"{PostgreSqlOptionsSection.SectionName}:{nameof(PostgreSqlOptionsSection.Schema)}", value: schema)
             .UseSetting(key: $"GrarOptions:Sqs:AddressMatchQueueName", value: schema.ToLowerInvariant());
        };
    }
}
