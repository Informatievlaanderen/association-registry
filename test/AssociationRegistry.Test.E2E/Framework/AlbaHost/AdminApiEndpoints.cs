namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Admin.Api.Administratie.Configuratie;
using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Search.ResponseModels;
using Alba;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Xunit;

public static class AdminApiEndpoints
{
    public static async Task<HistoriekResponse> GetBeheerHistoriek(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode,
        RequestParameters? headers = null)
        => SmartHttpClient
          .Create(source, authenticatedClient, headers)
          .GetWithRetryAsync<HistoriekResponse>($"/v1/verenigingen/{vCode}/historiek")
          .GetAwaiter()
          .GetResult();

    public static async Task<DetailVerenigingResponse> GetBeheerDetail(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode,
        RequestParameters? headers = null)
        => SmartHttpClient
          .Create(source, authenticatedClient, headers)
          .GetWithRetryAsync<DetailVerenigingResponse>($"/v1/verenigingen/{vCode}")
          .GetAwaiter()
          .GetResult();


    public static ProblemDetails GetProblemDetailsForBeheerDetailHttpResponse(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode,
        long expectedSequence,
        RequestParameters? headers = null)
        => SmartHttpClient
          .Create(source, authenticatedClient, headers)
          .GetWithRetryAsync<ProblemDetails>($"/v1/verenigingen/{vCode}?expectedSequence={expectedSequence}")
          .GetAwaiter()
          .GetResult();

    public static async Task<MinimumScoreDuplicateDetectionOverrideResponse> GetMinimumScoreDuplicateDetectionOverride(
        this IAlbaHost source,
        HttpClient authenticatedClient,
         RequestParameters? headers = null)
        => await SmartHttpClient
                .Create(source, authenticatedClient, headers)
                .GetWithRetryAsync<MinimumScoreDuplicateDetectionOverrideResponse>("/v1/admin/config/minimumScoreDuplicateDetection");

    public static async Task<HttpResponseMessage> PostMinimumScoreDuplicateDetectionOverride(
        this IAlbaHost source,
        OverrideMinimumScoreDuplicateDetectionRequest request,
        HttpClient authenticatedClient)
        => await SmartHttpClient
                .Create(source, authenticatedClient)
                .PostAsync("/v1/admin/config/minimumScoreDuplicateDetection", JsonContent.Create(request));

    public static async Task<SearchVerenigingenResponse> GetBeheerZoeken(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string query,
        RequestParameters? headers = null)
        => await SmartHttpClient.Create(source, authenticatedClient, headers).GetWithRetryAsync<SearchVerenigingenResponse>(
            $"/v1/verenigingen/zoeken?q={HttpUtility.UrlEncode(query)}");

    public static HttpClient CreateClientWithHeaders(this IAlbaHost source, HttpClient baseClient)
        => SmartHttpClient.Create(source, baseClient).HttpClient;
}

public class SmartHttpClient
{
    private readonly HttpClient _client;
    private readonly RequestParameters.Result? _requestParameters;

    private SmartHttpClient(HttpClient client, RequestParameters.Result? requestParameters)
    {
        _client = client;
        _requestParameters = requestParameters;
    }

    public HttpClient HttpClient => _client;

    public async Task<TResponse> GetWithRetryAsync<TResponse>(string uri)
    {
        if (_requestParameters is not null)
            uri = EmbellishUri(uri, _requestParameters);

        const int maxRetries = 5;
        var delay = TimeSpan.FromMilliseconds(300);

        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            var response = await _client.GetAsync(uri);

            if (response.StatusCode is not HttpStatusCode.PreconditionFailed or HttpStatusCode.NotFound)
            {
                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResponse>(json)!;
            }

            await Task.Delay(delay);
            delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
        }

        throw new HttpRequestException($"Failed to retrieve {uri} after {maxRetries} retries due to 412 responses.");
    }

    private static string EmbellishUri(string uri, RequestParameters.Result requestParameters)
    {
        if (requestParameters.ExpectedSequence is null)
            return uri;

        if (uri.Contains(WellknownParameters.ExpectedSequence))
            throw new InvalidOperationException("ExpectedSequence already in uri");

        if (uri.Contains("?"))
            uri += $"&{WellknownParameters.ExpectedSequence}={requestParameters.ExpectedSequence}";
        else
            uri += $"?{WellknownParameters.ExpectedSequence}={requestParameters.ExpectedSequence}";

        return uri;
    }

    public static SmartHttpClient Create(IAlbaHost host, HttpClient? baseClient, RequestParameters? requestParametersBuilder = null)
    {
        var requestParameters = requestParametersBuilder?.Build();
        var client = host.Server.CreateClient();

        if (baseClient != null)
        {
            AddHeaders(client, baseClient.DefaultRequestHeaders);
        }

        if (requestParameters is null)
            return new(client, null);

        AddHeaders(client, requestParameters.Headers);

        return new(client, requestParameters);
    }

    private static void AddHeaders(
        HttpClient client,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> baseClientDefaultRequestHeaders)
    {
        foreach (var header in baseClientDefaultRequestHeaders)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    public async Task<HttpResponseMessage> PostAsync(string uri, JsonContent create)
        => await _client.PostAsync(uri, create);
}

public class RequestParameters
{
    private readonly Dictionary<string, string[]> _headers = new();
    private long? _expectedSequence;

    public RequestParameters With(string key, string value)
    {
        _headers[key] = [value];
        _expectedSequence = null;
        return this;
    }

    public RequestParameters V2()
        => With(WellknownHeaderNames.Version, WellknownVersions.V2);

    public RequestParameters WithExpectedSequence(long? expectedSequence)
    {
        _expectedSequence = expectedSequence;

        return this;
    }

    public Result Build() => new(
        _headers
           .ToDictionary(
                x => x.Key,
                x => x.Value)
           .Select(
                x => new KeyValuePair<string, IEnumerable<string>>(
                    x.Key,
                    x.Value)),
        _expectedSequence);

    public record Result(IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers, long? ExpectedSequence);
}


public static class Headers
{
    public static RequestParameters None()
        => new();

    public static RequestParameters V2()
        => new RequestParameters().V2();
}
