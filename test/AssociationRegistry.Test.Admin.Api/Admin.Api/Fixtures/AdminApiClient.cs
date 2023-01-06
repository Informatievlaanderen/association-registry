namespace AssociationRegistry.Test.Admin.Api.Admin.Api.Fixtures;

using AssociationRegistry.Admin.Api.Infrastructure;
using Framework.Helpers;

public class AdminApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public AdminApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetDetail(string vCode, long? expectedSequence = null)
        => await GetWithPossibleSequence($"/v1/verenigingen/{vCode}", expectedSequence);

    public async Task<HttpResponseMessage> GetHistoriek(string vCode, long? expectedSequence = null)
        => await GetWithPossibleSequence($"/v1/verenigingen/{vCode}/historiek", expectedSequence);

    public async Task<HttpResponseMessage> RegistreerVereniging(string content)
        => await _httpClient.PostAsync("/v1/verenigingen", content.AsJsonContent());

    private async Task<HttpResponseMessage> GetWithPossibleSequence(string? requestUri, long? expectedSequence)
        => expectedSequence == null ?
            await _httpClient.GetAsync(requestUri) :
            await _httpClient.GetAsync($"{requestUri}?{WellknownParameters.ExpectedSequence}={expectedSequence}");

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
