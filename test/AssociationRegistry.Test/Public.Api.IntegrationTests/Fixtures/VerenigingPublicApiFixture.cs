using System.Reflection;
using AssociationRegistry.Public.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using AssociationRegistry.Public.Api.S3;
using Be.Vlaanderen.Basisregisters.BlobStore;
using Microsoft.Extensions.DependencyInjection;
using Tests;

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

        var blobClient = _testServer.Services.GetRequiredService<VerenigingenBlobClient>();
        var associatedResourceJson = typeof(Scenario).GetAssociatedResourceJson("list-verenigingen-context");
        if (!blobClient.BlobExistsAsync(WellknownBuckets.Verenigingen.Blobs.ListVerenigingenContext).GetAwaiter().GetResult())
        {
            blobClient.CreateBlobAsync(WellknownBuckets.Verenigingen.Blobs.ListVerenigingenContext, Metadata.None,
                ContentType.Parse("application/json"),
                GenerateStreamFromString(associatedResourceJson)).GetAwaiter().GetResult();
        }

        HttpClient = _testServer.CreateClient();
    }

    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        _testServer.Dispose();
    }
}
