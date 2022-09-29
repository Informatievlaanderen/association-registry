namespace AssociationRegistry.Test.Acm.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Acm.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class VerenigingAcmApiFixture : IDisposable
{
    public IConfigurationRoot? ConfigurationRoot { get; }
    public TestServer Server { get; }

    public VerenigingAcmApiFixture()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        Directory.SetCurrentDirectory(rootDirectory);

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        ConfigurationRoot = builder.Build();

        IWebHostBuilder hostBuilder = new WebHostBuilder();

        hostBuilder.UseConfiguration(ConfigurationRoot);
        hostBuilder.UseStartup<Startup>();

        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());

        hostBuilder.UseTestServer();

        Server = new TestServer(hostBuilder);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Server.Dispose();
    }
}
