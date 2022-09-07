using System.Reflection;
using AssociationRegistry.Public.Api;
using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_verenigingen;

using Newtonsoft.Json.Linq;

public class Given_an_api:IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly TestServer _testServer;

    public Given_an_api()
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

        _httpClient = _testServer.CreateClient();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _httpClient.GetAsync("/v1/verenigingen")).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_list_verenigingen_response()
    {
        var responseMessage = await _httpClient.GetAsync("/v1/verenigingen");
        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(nameof(Then_we_get_a_list_verenigingen_response));

        var deserializedContent = JToken.Parse(content);
        var deserializedGoldenMaster = JToken.Parse(goldenMaster);

        deserializedContent.Should().BeEquivalentTo(deserializedGoldenMaster);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _testServer.Dispose();
    }
}
