﻿namespace AssociationRegistry.Test.E2E;

using Admin.Api;
using Admin.Api.Infrastructure.Extensions;
using Alba;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Configuration;
using Framework.AlbaHost;
using Hosts;
using Hosts.Configuration.ConfigurationBindings;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oakton;
using Xunit;
using Clients = Common.Clients.Clients;
using ProjectionHostProgram = Admin.ProjectionHost.Program;

public class AdminApiSetup : IApiSetup
{
    public string? AuthCookie { get; private set; }
    public ILogger<Program> Logger { get; private set; }
    public IAlbaHost AdminApiHost { get; private set; }
    public IAlbaHost ProjectionHost { get; private set; }
    public IAlbaHost QueryApiHost => AdminApiHost;

    public AdminApiSetup()
    {
    }

    public async Task InitializeAsync(string schema)
    {
        schema = schema.ToLowerInvariant();
        OaktonEnvironment.AutoStartHost = true;

        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.json").Build();

        AdminApiHost = (await AlbaHost.For<Program>(ConfigureForTesting(configuration, schema)))
           .EnsureEachCallIsAuthenticated();

        Logger = AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

        ProjectionHost = await AlbaHost.For<ProjectionHostProgram>(ConfigureForTesting(configuration, schema));

        await AdminApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await ProjectionHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        await QueryApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await ProjectionHost.ResumeAllDaemonsAsync();
    }

    private Action<IWebHostBuilder> ConfigureForTesting(IConfigurationRoot configuration, string schema)
    {
        var sqsConfig = new AmazonSQSConfig
        {
            ServiceURL = "http://localhost:4566", // LocalStack endpoint
            UseHttp = true,
            AuthenticationRegion = "eu-west-1" // Region for LocalStack
        };

        IAmazonSQS sqs = new AmazonSQSClient(new BasicAWSCredentials("dummy", "dummy"), sqsConfig);

        var createQueueRequest = new CreateQueueRequest
        {
            QueueName = schema // Assuming schema is a variable holding the queue name
        };

        var sqsResult = sqs.CreateQueueAsync(createQueueRequest).GetAwaiter().GetResult();

        sqs.PurgeQueueAsync(schema).GetAwaiter().GetResult();

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

    public async Task DisposeAsync()
    {
        await AdminApiHost.Services.GetRequiredService<IDocumentStore>().Advanced.ResetAllData();
        await AdminApiHost.DisposeAsync();
    }
}
