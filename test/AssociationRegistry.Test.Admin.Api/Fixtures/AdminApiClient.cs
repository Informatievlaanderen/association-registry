namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using Framework.Helpers;
using global::AssociationRegistry.Admin.Api.Infrastructure;
using Microsoft.Net.Http.Headers;

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
        => expectedSequence == null ? await _httpClient.GetAsync(requestUri) : await _httpClient.GetAsync($"{requestUri}?{WellknownParameters.ExpectedSequence}={expectedSequence}");

    public async Task<HttpResponseMessage> PatchVereniging(string vCode, string content, long? version = null)
    {
        SetIfMatchHeader(version);
        return await _httpClient.PatchAsync($"/v1/verenigingen/{vCode}", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> GetDocs()
        => await _httpClient.GetAsync($"/docs/v1/docs.json?culture=en-GB");

    private void SetIfMatchHeader(long? version)
    {
        if (version is null) _httpClient.DefaultRequestHeaders.Remove(HeaderNames.IfMatch);
        else _httpClient.DefaultRequestHeaders.Add(HeaderNames.IfMatch, $"W/\"{version}\"");
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
