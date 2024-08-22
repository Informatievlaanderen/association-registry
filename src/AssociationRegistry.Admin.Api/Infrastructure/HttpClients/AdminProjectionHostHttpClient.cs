namespace AssociationRegistry.Admin.Api.Infrastructure.HttpClients;

public class AdminProjectionHostHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public AdminProjectionHostHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> RebuildAllProjections(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/all/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildDetailProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/detail/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildLocatieLookupProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/locaties/lookup/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildLocatieZonderAdresMatchProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "v1/projections/locaties/zonderadresmatch/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildHistoriekProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/historiek/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildZoekenProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/search/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildDuplicateDetectionProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/duplicatedetection/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> GetStatus(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri: "/v1/projections/status");
        request.Headers.Add(name: "X-Correlation-Id", Guid.NewGuid().ToString());

        return await _httpClient.SendAsync(request, cancellationToken);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
