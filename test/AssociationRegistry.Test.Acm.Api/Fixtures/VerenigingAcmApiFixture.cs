namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using AssociationRegistry.Acm.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

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
                               .AddJsonFile(path: "appsettings.json", optional: true)
                               .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                    );

                    builder.ConfigureServices(
                        (context, services) => { services.AddSingleton(context.Configuration); });
                });
    }

    private static string GetRootDirectoryOrThrow()
    {
        var maybeRootDirectory = Directory
                                .GetParent(Assembly.GetExecutingAssembly().Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        return rootDirectory;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Server.Dispose();
    }
}
