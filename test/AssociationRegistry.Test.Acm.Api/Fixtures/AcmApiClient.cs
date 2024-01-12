namespace AssociationRegistry.Test.Acm.Api.Fixtures;

public class AcmApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public AcmApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetRoot()
        => await _httpClient.GetAsync("");

    public async Task<HttpResponseMessage> GetDocsJson()
        => await _httpClient.GetAsync("/docs/v1/docs.json?culture=en-GB");

    public async Task<HttpResponseMessage> GetVerenigingenForInsz(string insz)
        => await _httpClient.GetAsync($"/v1/verenigingen?insz={insz}");

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
