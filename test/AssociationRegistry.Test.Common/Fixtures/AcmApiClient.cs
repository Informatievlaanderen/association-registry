namespace AssociationRegistry.Test.Common.Fixtures;

public class AcmApiClient : IDisposable
{
    public HttpClient HttpClient { get; }

    public AcmApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetRoot()
        => await HttpClient.GetAsync("");

    public async Task<HttpResponseMessage> GetDocsJson()
        => await HttpClient.GetAsync("/docs/v1/docs.json?culture=en-GB");

    public async Task<HttpResponseMessage> GetVerenigingenForInsz(string insz, bool includeKboVerenigingen = false)
        => await HttpClient.GetAsync($"/v1/verenigingen?insz={insz}&includeKboVerenigingen={includeKboVerenigingen}");

    public void Dispose()
    {
        HttpClient.Dispose();
    }
}
