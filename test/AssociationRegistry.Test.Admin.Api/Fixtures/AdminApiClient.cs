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

    public async Task<HttpResponseMessage> RegistreerFeitelijkeVereniging(string content, string? bevestigingsToken = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken, bevestigingsToken);
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        var httpResponseMessage = await _httpClient.PostAsync("/v1/verenigingen", content.AsJsonContent());
        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> RegistreerAfdeling(string content, string? bevestigingsToken = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken, bevestigingsToken);
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        var httpResponseMessage = await _httpClient.PostAsync("/v1/verenigingen/afdelingen", content.AsJsonContent());
        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken);
        return httpResponseMessage;
    }
    public async Task<HttpResponseMessage> RegistreerKboVereniging(string content, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        var httpResponseMessage = await _httpClient.PostAsync($"/v1/verenigingen/kbo", content.AsJsonContent());
        return httpResponseMessage;

    }

    private async Task<HttpResponseMessage> GetWithPossibleSequence(string? requestUri, long? expectedSequence)
        => expectedSequence == null ? await _httpClient.GetAsync(requestUri) : await _httpClient.GetAsync($"{requestUri}?{WellknownParameters.ExpectedSequence}={expectedSequence}");

    public async Task<HttpResponseMessage> PatchVereniging(string vCode, string content, long? version = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        return await _httpClient.PatchAsync($"/v1/verenigingen/{vCode}", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchVerenigingMetRechtspersoonlijkheid(string vCode, string content, long? version = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        return await _httpClient.PatchAsync($"/v1/verenigingen/kbo/{vCode}", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PostVertegenwoordiger(string vCode, string content, long? version = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        return await _httpClient.PostAsync($"/v1/verenigingen/{vCode}/vertegenwoordigers", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PostContactgegevens(string vCode, string content, long? version = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        return await _httpClient.PostAsync($"/v1/verenigingen/{vCode}/contactgegevens", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchContactgegevens(string vCode, int contactgegevenId, string jsonBody, long? version = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        return await _httpClient.PatchAsync($"/v1/verenigingen/{vCode}/contactgegevens/{contactgegevenId}", jsonBody.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchVertegenwoordiger(string vCode, int vertegenwoordigerId, string jsonBody, long? version = null, string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        AddOrRemoveHeader(WellknownHeaderNames.Initiator, initiator);
        return await _httpClient.PatchAsync($"/v1/verenigingen/{vCode}/vertegenwoordigers/{vertegenwoordigerId}", jsonBody.AsJsonContent());
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

    public async Task<HttpResponseMessage> GetHoofdactiviteiten()
        => await _httpClient.GetAsync($"/v1/hoofdactiviteitenVerenigingsloket");

    public async Task<HttpResponseMessage> GetJsonLdContext(string contextName)
        => await _httpClient.GetAsync($"/v1/contexten/{contextName}");
}
