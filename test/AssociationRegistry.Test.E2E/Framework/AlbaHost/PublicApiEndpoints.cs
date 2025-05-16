namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Alba;
using Marten;
using Marten.Events.Daemon;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Public.Api.Infrastructure;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Public.Api.Verenigingen.Mutaties;
using Public.Api.Verenigingen.Search.ResponseModels;
using Public.Api.Werkingsgebieden.ResponseModels;
using System.Net;

public static class PublicApiEndpoints
{
    public static PubliekVerenigingDetailResponse GetPubliekDetail(this IAlbaHost source, string vCode)
        => source.GetAsJson<PubliekVerenigingDetailResponse>($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult()!;

    public static async Task<PubliekVerenigingDetailResponse> GetPubliekDetailWithHeader(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode)
        => await GetResponseFromRequestWithHeader<PubliekVerenigingDetailResponse>(source, authenticatedClient, $"/v1/verenigingen/{vCode}");

    public static HttpStatusCode GetPubliekDetailStatusCode(this IAlbaHost source, string vCode)
    {
        var client = source.Server.CreateClient();
        var response = client.GetAsync($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult();

        return response.StatusCode;
    }

    public static PubliekVerenigingSequenceResponse[] GetVerenigingMutationsSequence(this IAlbaHost source)
    {
        return source.GetAsJson<PubliekVerenigingSequenceResponse[]>($"/v1/verenigingen/mutaties").GetAwaiter().GetResult()!;
    }

    public static SearchVerenigingenResponse GetPubliekZoeken(this IAlbaHost source, string query)
    {
        return source.GetAsJson<SearchVerenigingenResponse>($"/v1/verenigingen/zoeken?q={query}").GetAwaiter().GetResult()!;
    }

    public static async Task<SearchVerenigingenResponse> GetPubliekZoekenWithHeader(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string query,
        long? expectedSequence)
    {
        await WaitForExpectedSequence(source, expectedSequence);
        return await GetResponseFromRequestWithHeader<SearchVerenigingenResponse>(source, authenticatedClient,
                                                                                  $"/v1/verenigingen/zoeken?q={query}");
    }

    private static async Task WaitForExpectedSequence(IAlbaHost source, long? expectedSequence)
    {
        var store = source.Services.GetRequiredService<IDocumentStore>();
        await source.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
        var result = (await store.Advanced
                                  .AllProjectionProgress()).SingleOrDefault(x => x.ShardName == "PubliekVerenigingZoekenDocument:All")?.Sequence;


        bool reachedSequence = result >= expectedSequence;
        var counter = 0;
        while (!reachedSequence && counter < 10)
        {
            counter++;
            await Task.Delay(500);
            await source.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
            result = (await store.Advanced
                                  .AllProjectionProgress()).SingleOrDefault(x => x.ShardName == "PubliekVerenigingZoekenDocument:All")?.Sequence;

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

    public static WerkingsgebiedenResponse GetWerkingsgebieden(this IAlbaHost source)
        => source.GetAsJson<WerkingsgebiedenResponse>($"/v1/werkingsgebieden").GetAwaiter().GetResult()!;

    public static JObject[] GetPubliekDetailAll(this IAlbaHost source)
    {
        var locationHeader = GetDetailAllLocationHeader(source);

        var s3Response = GetS3Response(locationHeader);

        var responseContent = s3Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

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

    private static HttpResponseMessage GetS3Response(Uri? locationHeader)
    {
        using var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        using var s3HttpClient = new HttpClient(handler);

        return s3HttpClient.GetAsync(locationHeader).GetAwaiter().GetResult();
    }

    private static Uri? GetDetailAllLocationHeader(IAlbaHost source)
    {
        var client = source.Server.CreateClient();
        var response = client.GetAsync($"/v1/verenigingen/detail/all").GetAwaiter().GetResult();

        return response.Headers.Location;
    }
}
