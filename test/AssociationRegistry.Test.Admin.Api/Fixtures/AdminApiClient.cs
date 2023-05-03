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

    public async Task<HttpResponseMessage> GetRoot()
        => await _httpClient.GetAsync("");

    public async Task<HttpResponseMessage> GetDetail(string vCode, long? expectedSequence = null)
        => await GetWithPossibleSequence($"/v1/verenigingen/{vCode}", expectedSequence);

    public async Task<HttpResponseMessage> GetHistoriek(string vCode, long? expectedSequence = null)
        => await GetWithPossibleSequence($"/v1/verenigingen/{vCode}/historiek", expectedSequence);

    public async Task<HttpResponseMessage> RegistreerVereniging(string content, string? bevestigingsToken = null)
    {
        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken, bevestigingsToken);
        var httpResponseMessage = await _httpClient.PostAsync("/v1/verenigingen", content.AsJsonContent());
        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken);
        return httpResponseMessage;
    }

    private async Task<HttpResponseMessage> GetWithPossibleSequence(string? requestUri, long? expectedSequence)
        => expectedSequence == null ? await _httpClient.GetAsync(requestUri) : await _httpClient.GetAsync($"{requestUri}?{WellknownParameters.ExpectedSequence}={expectedSequence}");

    public async Task<HttpResponseMessage> PatchVereniging(string vCode, string content, long? version = null)
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        return await _httpClient.PatchAsync($"/v1/verenigingen/{vCode}", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PostVertegenwoordiger(string vCode, string content, long? version = null)
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        return await _httpClient.PostAsync($"/v1/verenigingen/{vCode}/vertegenwoordigers", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PostContactgegevens(string vCode, string content, long? version = null)
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        return await _httpClient.PostAsync($"/v1/verenigingen/{vCode}/contactgegevens", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchContactgegevens(string vCode, int contactgegevenId, string jsonBody, long? version = null)
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        return await _httpClient.PatchAsync($"/v1/verenigingen/{vCode}/contactgegevens/{contactgegevenId}", jsonBody.AsJsonContent());
    }

    public async Task<HttpResponseMessage> DeleteContactgegeven(string vCode, int contactgegevenId, long? version = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"/v1/verenigingen/{vCode}/contactgegevens/{contactgegevenId}", UriKind.Relative),
        };
        return await _httpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> DeleteVertegenwoordiger(string vCode, int vertegenwoordigerId, string jsonBody, long? version = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"/v1/verenigingen/{vCode}/vertegenwoordigers/{vertegenwoordigerId}", UriKind.Relative),
        };
        return await _httpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> GetDocsJson()
        => await _httpClient.GetAsync($"/docs/v1/docs.json?culture=en-GB");

    private static string? GetIfMatchHeaderValue(long? version)
        => version is not null ? $"W/\"{version}\"" : null;

    private void AddOrRemoveHeader(string headerName, string? headerValue = null)
    {
        _httpClient.DefaultRequestHeaders.Remove(headerName);
        if (headerValue is not null) _httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
