namespace AssociationRegistry.Integrations.Grar.Clients;

using Microsoft.AspNetCore.WebUtilities;

public class GrarHttpClient : IGrarHttpClient
{
    private readonly HttpClient _httpClient;

    public GrarHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetAddressById(string adresId, CancellationToken cancellationToken)
        => await _httpClient.GetAsync($"/v2/adressen/{adresId}", cancellationToken);

    public async Task<HttpResponseMessage> GetAddressMatches(
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeentenaam,
        CancellationToken cancellationToken)
    {
        var queryDict = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(straatnaam)) queryDict.Add("Straatnaam", straatnaam);
        if (!string.IsNullOrEmpty(huisnummer)) queryDict.Add("Huisnummer", huisnummer);
        if (!string.IsNullOrEmpty(busnummer)) queryDict.Add("Busnummer", busnummer);
        if (!string.IsNullOrEmpty(postcode)) queryDict.Add("Postcode", postcode);
        if (!string.IsNullOrEmpty(gemeentenaam)) queryDict.Add("Gemeentenaam", gemeentenaam);

        var requestUri = $"/v2/adresmatch?{string.Join('&', queryDict.Select(queryItem => $"{queryItem.Key}={queryItem.Value}"))}";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        return response;
    }

    public async Task<HttpResponseMessage> GetPostInfoDetail(string postcode, CancellationToken cancellationToken)
        => await _httpClient.GetAsync($"/v2/postinfo/{postcode}", cancellationToken);

    public async Task<HttpResponseMessage> GetPostInfoList(string? offset, string? limit, CancellationToken cancellationToken)
    {
        var url = UrlBuilder.Build("/v2/postinfo", offset, limit);

        return await _httpClient.GetAsync(url, cancellationToken);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

public static class UrlBuilder
{
    public static string Build(string baseUrl, string? offset, string? limit)
    {
        var queryParams = new Dictionary<string, string?>();

        if (!string.IsNullOrEmpty(offset))
            queryParams["offset"] = offset;

        if (!string.IsNullOrEmpty(limit))
            queryParams["limit"] = limit;

        return QueryHelpers.AddQueryString(baseUrl, queryParams);
    }
}
