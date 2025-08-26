namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Alba;
using Marten;
using Marten.Events.Daemon;
using Microsoft.Extensions.DependencyInjection;
using Elastic.Clients.Elasticsearch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Public.Api.Infrastructure;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Public.Api.WebApi.Verenigingen.Mutaties;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Public.Api.WebApi.Werkingsgebieden.ResponseModels;
using System.Net;

public static class PublicApiEndpoints
{
    public static async Task<PubliekVerenigingDetailResponse> GetPubliekDetail(this IAlbaHost source, string vCode)
        => await source.GetAsJson<PubliekVerenigingDetailResponse>($"/v1/verenigingen/{vCode}");

    public static async Task<PubliekVerenigingDetailResponse> GetPubliekDetailWithHeader(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode)
        => await GetResponseFromRequestWithHeader<PubliekVerenigingDetailResponse>(source, authenticatedClient, $"/v1/verenigingen/{vCode}");

    public static async Task<HttpStatusCode> GetPubliekDetailStatusCode(this IAlbaHost source, string vCode, long? expectedSequence)
    {
        await WaitForExpectedSequence(source, expectedSequence, "AssociationRegistry.Public.ProjectionHost.Projections.Detail.PubliekVerenigingDetailProjection:All");

        var client = source.Server.CreateClient();
        var response = await client.GetAsync($"/v1/verenigingen/{vCode}");

        return response.StatusCode;
    }

    public static async Task<PubliekVerenigingSequenceResponse[]> GetVerenigingMutationsSequence(this IAlbaHost source)
    {
        return await source.GetAsJson<PubliekVerenigingSequenceResponse[]>($"/v1/verenigingen/mutaties");
    }

    public static async Task<SearchVerenigingenResponse> GetPubliekZoeken(this IAlbaHost source, string query, long? expectedSequence)
    {
        await WaitForExpectedSequence(source, expectedSequence, "PubliekVerenigingZoekenDocument:All");

        return await source.GetAsJson<SearchVerenigingenResponse>($"/v1/verenigingen/zoeken?q={query}");
    }

    public static async Task<SearchVerenigingenResponse> GetPubliekZoekenWithHeader(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string query,
        long? expectedSequence)
    {
        await WaitForExpectedSequence(source, expectedSequence, "PubliekVerenigingZoekenDocument:All");
        return await GetResponseFromRequestWithHeader<SearchVerenigingenResponse>(source, authenticatedClient,
                                                                                  $"/v1/verenigingen/zoeken?q={query}");
    }

    private static async Task WaitForExpectedSequence(IAlbaHost source, long? expectedSequence, string publiekverenigingzoekendocumentAll)
    {
        var store = source.Services.GetRequiredService<IDocumentStore>();
        await source.Services.GetRequiredService<ElasticsearchClient>().Indices.RefreshAsync(Indices.All);
        var result = (await store.Advanced
                                  .AllProjectionProgress()).SingleOrDefault(x => x.ShardName == publiekverenigingzoekendocumentAll)?.Sequence;


        bool reachedSequence = result >= expectedSequence;
        var counter = 0;
        while (!reachedSequence && counter < 10)
        {
            counter++;
            await Task.Delay(500);
            await source.Services.GetRequiredService<ElasticsearchClient>().Indices.RefreshAsync(Indices.All);
            result = (await store.Advanced
                                  .AllProjectionProgress()).SingleOrDefault(x => x.ShardName == publiekverenigingzoekendocumentAll)?.Sequence;

            reachedSequence = result >= expectedSequence;
        }
    }

    private static async Task<TResponse> GetResponseFromRequestWithHeader<TResponse>(
        IAlbaHost source,
        HttpClient authenticatedClient,
        string requestUri)
    {
        var client = source.Server.CreateClient();

        foreach (var defaultRequestHeader in authenticatedClient.DefaultRequestHeaders)
        {
            client.DefaultRequestHeaders.Add(defaultRequestHeader.Key, defaultRequestHeader.Value);
        }

        client.DefaultRequestHeaders.Add(WellknownHeaderNames.Version, WellknownVersions.V2);

        var response = await client.GetAsync(requestUri);

        while (response.StatusCode == HttpStatusCode.PreconditionFailed)
        {
            await Task.Delay(300);
            response = await client.GetAsync(requestUri);
        }

        var readAsStringAsync = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TResponse>(readAsStringAsync);
    }

    public static async Task<WerkingsgebiedenResponse> GetWerkingsgebieden(this IAlbaHost source)
        => await source.GetAsJson<WerkingsgebiedenResponse>($"/v1/werkingsgebieden");

    public static async Task<JObject[]> GetPubliekDetailAll(this IAlbaHost source, long? expectedSequence)
    {
        await WaitForExpectedSequence(source, expectedSequence, "AssociationRegistry.Public.ProjectionHost.Projections.Detail.PubliekVerenigingDetailProjection:All");

        var locationHeader = await GetDetailAllLocationHeader(source);

        var s3Response = await GetS3Response(locationHeader);

        var responseContent = await s3Response.Content.ReadAsStringAsync();

        return ParseResponse(responseContent);
    }

    private static JObject[] ParseResponse(string responseContent)
    {
        if (string.IsNullOrEmpty(responseContent))
            return [];

        var verenigingen = new List<JObject>();
        var reader = new StringReader(responseContent);

        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            verenigingen.Add(JObject.Parse(line));
        }

        return verenigingen.ToArray();
    }

    private static async Task<HttpResponseMessage> GetS3Response(Uri? locationHeader)
    {
        using var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        using var s3HttpClient = new HttpClient(handler);

        return await s3HttpClient.GetAsync(locationHeader);
    }

    private static async Task<Uri?> GetDetailAllLocationHeader(IAlbaHost source)
    {
        var client = source.Server.CreateClient();
        var response = await client.GetAsync($"/v1/verenigingen/detail/all");

        return response.Headers.Location;
    }
}
