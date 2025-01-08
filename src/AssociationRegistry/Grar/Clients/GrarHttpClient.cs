namespace AssociationRegistry.Grar.Clients;

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

    public async Task<HttpResponseMessage> GetPostInfo(string postcode, CancellationToken cancellationToken)
    {
        var requestUri = $"/v2/postinfo/{postcode}";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        return response;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
