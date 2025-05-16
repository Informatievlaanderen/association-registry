namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Admin.Api.Administratie.Configuratie;
using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Search.ResponseModels;
using Alba;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Web;
using Xunit;

public static class AdminApiEndpoints
{
    public static async Task<HistoriekResponse> GetBeheerHistoriek(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode,
        RequestHeadersBuilder? headers = null)
        => await GetWithRetryAsync<HistoriekResponse>(
            source,
            authenticatedClient,
            $"/v1/verenigingen/{vCode}/historiek",
            null,
            headers);

    public static async Task<string> GetDetailAsText(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode,
        RequestHeadersBuilder? headers = null)
    {
        var uri = $"/v1/verenigingen/{vCode}";
        var client = source.CreateClientWithHeaders(authenticatedClient, headers);

        var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<DetailVerenigingResponse> GetBeheerDetail(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode,
        RequestHeadersBuilder? headers = null)
        => await GetWithRetryAsync<DetailVerenigingResponse>(
            source,
            authenticatedClient,
            $"/v1/verenigingen/{vCode}",
            null,
            headers);

    public static HttpResponseMessage GetBeheerDetailHttpResponse(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode,
        long expectedSequence,
        RequestHeadersBuilder? headers = null)
    {
        var uri = $"/v1/verenigingen/{vCode}?expectedSequence={expectedSequence}";
        var client = source.CreateClientWithHeaders(authenticatedClient, headers);
        return client.GetAsync(uri).GetAwaiter().GetResult();
    }

    public static async Task<MinimumScoreDuplicateDetectionOverrideResponse> GetMinimumScoreDuplicateDetectionOverride(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        ITestOutputHelper? helper = null,
        RequestHeadersBuilder? headers = null)
        => await GetWithRetryAsync<MinimumScoreDuplicateDetectionOverrideResponse>(
            source,
            authenticatedClient,
            "/v1/admin/config/minimumScoreDuplicateDetection",
            helper,
            headers);

    public static async Task<HttpResponseMessage> PostMinimumScoreDuplicateDetectionOverride(
        this IAlbaHost source,
        OverrideMinimumScoreDuplicateDetectionRequest request,
        HttpClient authenticatedClient,
        RequestHeadersBuilder? headers = null)
    {
        var client = source.CreateClientWithHeaders(authenticatedClient, headers);
        var uri = "/v1/admin/config/minimumScoreDuplicateDetection";
        return await client.PostAsync(uri, JsonContent.Create(request));
    }

    public static async Task<SearchVerenigingenResponse> GetBeheerZoeken(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string query,
        ITestOutputHelper? helper = null,
        RequestHeadersBuilder? headers = null)
        => await GetWithRetryAsync<SearchVerenigingenResponse>(
            source,
            authenticatedClient,
            $"/v1/verenigingen/zoeken?q={HttpUtility.UrlEncode(query)}",
            helper,
            headers);
    // ---------- Shared helpers ----------

    private static async Task<TResponse> GetWithRetryAsync<TResponse>(
        IAlbaHost source,
        HttpClient authenticatedClient,
        string uri,
        ITestOutputHelper? helper = null,
        RequestHeadersBuilder? headers = null)
    {
        var client = source.CreateClientWithHeaders(authenticatedClient, headers);
        const int maxRetries = 5;
        var delay = TimeSpan.FromMilliseconds(300);

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            var response = await client.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.PreconditionFailed)
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(json)!;
            }

            await Task.Delay(delay);
            delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
        }

        throw new HttpRequestException($"Failed to retrieve {uri} after {maxRetries} retries due to 412 responses.");
    }

    public static HttpClient CreateClientWithHeaders(
        this IAlbaHost source,
        HttpClient? baseClient = null,
        RequestHeadersBuilder? builder = null)
    {
        var client = source.Server.CreateClient();

        if (baseClient != null)
        {
            foreach (var header in baseClient.DefaultRequestHeaders)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        foreach (var header in (builder ?? Headers.None()).Build())
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        return client;
    }
}

public class RequestHeadersBuilder
{
    private readonly Dictionary<string, string> _headers = new();

    public RequestHeadersBuilder With(string key, string value)
    {
        _headers[key] = value;
        return this;
    }

    public RequestHeadersBuilder V2()
        => With(WellknownHeaderNames.Version, WellknownVersions.V2);

    public RequestHeadersBuilder WithExpectedSequence(long? expectedSequence)
    {
        //TODO:
        if (expectedSequence.HasValue)
            With(WellknownHeaderNames., expectedSequence.Value.ToString());

        return this;
    }

    internal IEnumerable<KeyValuePair<string, string>> Build()
        => _headers;
}

public static class Headers
{
    public static RequestHeadersBuilder None()
        => new();

    public static RequestHeadersBuilder V2()
        => new RequestHeadersBuilder().V2();
}
