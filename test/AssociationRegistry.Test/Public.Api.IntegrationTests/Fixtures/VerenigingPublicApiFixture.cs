using System.Reflection;
using AssociationRegistry.Public.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

public class VerenigingPublicApiFixture : IDisposable
{
    public HttpClient HttpClient { get; private set; }
    private readonly TestServer _testServer;

    public VerenigingPublicApiFixture()
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

        var configurationRoot = builder.Build();

        IWebHostBuilder hostBuilder = new WebHostBuilder();

        hostBuilder.UseConfiguration(configurationRoot);
        hostBuilder.UseStartup<Startup>();
        hostBuilder.UseTestServer();

        _testServer = new TestServer(hostBuilder);

        HttpClient = _testServer.CreateClient();
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        _testServer.Dispose();
    }
}
