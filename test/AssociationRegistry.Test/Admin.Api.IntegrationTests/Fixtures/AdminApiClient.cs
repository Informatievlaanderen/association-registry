namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using AssociationRegistry.Admin.Api.Infrastructure;

public class AdminApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public AdminApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetDetail(string vCode, long? expectedSequence = null)
        => expectedSequence == null ?
            await _httpClient.GetAsync($"/v1/verenigingen/{vCode}") :
            await _httpClient.GetAsync($"/v1/verenigingen/{vCode}?{WellknownParameters.ExpectedSequence}={expectedSequence}");

    public async Task<HttpResponseMessage> GetHistoriek(string vCode)
        => await _httpClient.GetAsync($"/v1/verenigingen/{vCode}/historiek");

    public async Task<HttpResponseMessage> RegistreerVereniging(StringContent content)
        => await _httpClient.PostAsync("/v1/verenigingen", content);

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
