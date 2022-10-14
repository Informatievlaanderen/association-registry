namespace AssociationRegistry.Test.Acm.Api.IntegrationTests.Fixtures;

using System.Reflection;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using AssociationRegistry.Acm.Api;
using Be.Vlaanderen.Basisregisters.BlobStore.Aws;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        CreateS3BucketsIfNeeded();
    }

    private void CreateS3BucketsIfNeeded()
    {
        var amazonS3Client = Server.Services.GetRequiredService<AmazonS3Client>();
        var bucketName = ConfigurationRoot?["S3BlobClientOptions:Buckets:Verenigingen:Name"];

        var bucketExists = AmazonS3Util.DoesS3BucketExistV2Async(amazonS3Client, bucketName).GetAwaiter().GetResult();
        if (!bucketExists)
        {
            amazonS3Client.PutBucketAsync(
                new PutBucketRequest
                {
                    BucketName = bucketName,
                }).GetAwaiter().GetResult();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Server.Dispose();
    }
}
