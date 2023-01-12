namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using System.Reflection;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using global::AssociationRegistry.Acm.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class VerenigingAcmApiFixture : IDisposable
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public IConfiguration Configuration
        => _webApplicationFactory.Services.GetRequiredService<IConfiguration>();

    public TestServer Server
        => _webApplicationFactory.Server;

    public VerenigingAcmApiFixture()
    {
        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.ConfigureAppConfiguration(
                        cfg =>
                            cfg.SetBasePath(GetRootDirectoryOrThrow())
                                .AddJsonFile("appsettings.json", optional: true)
                                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                    );
                    builder.ConfigureServices(
                        (context, services) => { services.AddSingleton(context.Configuration); });
                });

        CreateS3BucketsIfNeeded();
    }

    private static string GetRootDirectoryOrThrow()
    {
        var maybeRootDirectory = Directory
            .GetParent(Assembly.GetExecutingAssembly().Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");
        return rootDirectory;
    }

    private void CreateS3BucketsIfNeeded()
    {
        var amazonS3Client = Server.Services.GetRequiredService<AmazonS3Client>();
        var bucketName = Configuration["S3BlobClientOptions:Buckets:Verenigingen:Name"];

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
