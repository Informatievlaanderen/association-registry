namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplication;

using ConfigurationBindings;
using Extensions;
using Hosts;
using Nest;
using Program = ProjectionHost.Program;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

public static class PrepareElasticSearch
{
    public static async Task EnsureElasticSearchIsInitialized(this WebApplication source)
    {
        var elasticClient = source.Services.GetRequiredService<IElasticClient>();
        var elasticSearchOptions = source.Services.GetRequiredService<ElasticSearchOptionsSection>();

        await WaitFor.ElasticSearchToBecomeAvailable(
            elasticClient, source.Services.GetRequiredService<ILogger<Program>>(), cancellationToken: CancellationToken.None);

        await EnsureIndexExists(elasticClient, elasticSearchOptions.Indices!.Verenigingen!);
    }

    private static async Task EnsureIndexExists(IElasticClient elasticClient, string verenigingenIndexName)
    {
        if (!(await elasticClient.Indices.ExistsAsync(verenigingenIndexName)).Exists)
        {
            var response = await elasticClient.Indices.CreateVerenigingIndexAsync(verenigingenIndexName);

            if (!response.IsValid)
                throw response.OriginalException;
        }
    }
}
