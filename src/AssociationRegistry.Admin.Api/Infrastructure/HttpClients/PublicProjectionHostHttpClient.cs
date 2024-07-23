namespace AssociationRegistry.Admin.Api.Infrastructure.HttpClients;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class PublicProjectionHostHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public PublicProjectionHostHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> RebuildAllProjections(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/all/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildDetailProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/detail/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildZoekenProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/search/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildSequenceProjection(CancellationToken cancellationToken)
        => await _httpClient.PostAsync(requestUri: "/v1/projections/sequence/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> GetStatus(CancellationToken cancellationToken)
        => await _httpClient.GetAsync(requestUri: "/v1/projections/status", cancellationToken);

    public void Dispose()
    {
        _httpClient.Dispose();
    }


}
