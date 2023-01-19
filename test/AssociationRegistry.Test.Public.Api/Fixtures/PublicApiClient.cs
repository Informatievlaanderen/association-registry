namespace AssociationRegistry.Test.Public.Api.Fixtures;

public class PublicApiClient : IDisposable
{
    public readonly HttpClient HttpClient;

    public PublicApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetDetail(string vCode)
        => await HttpClient.GetAsync($"/v1/verenigingen/{vCode}");

    public async Task<HttpResponseMessage> Search(string q)
        => await HttpClient.GetAsync($"/v1/verenigingen/zoeken?q={q}");

    public async Task<HttpResponseMessage> GetDocs()
        => await HttpClient.GetAsync($"/docs/v1/docs.json?culture=en-GB");

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}
