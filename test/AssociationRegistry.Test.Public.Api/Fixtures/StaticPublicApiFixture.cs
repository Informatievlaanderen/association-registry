namespace AssociationRegistry.Test.Public.Api.Fixtures;

using AssociationRegistry.Public.Api;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

public class StaticPublicApiFixture : IDisposable
{
    public HttpClient HttpClient { get; }
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    public IDocumentStore DocumentStore { get; }

    public StaticPublicApiFixture()
    {
        _webApplicationFactory = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(
                builder => { builder.UseConfiguration(GetConfiguration()); });

        HttpClient = _webApplicationFactory.CreateClient();
        DocumentStore = _webApplicationFactory.Services.GetRequiredService<IDocumentStore>();
    }

    private static IConfigurationRoot GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
                     .SetBasePath(GetRootDirectory())
                     .AddJsonFile(path: "appsettings.json", optional: true)
                     .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var configurationRoot = builder.Build();

        return configurationRoot;
    }

    private static string GetRootDirectory()
    {
        var maybeRootDirectory = Directory
                                .GetParent(typeof(Program).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        return rootDirectory;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        HttpClient.Dispose();
        _webApplicationFactory.Dispose();
    }
}
