namespace AssociationRegistry.Grar;

using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class GrarHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public GrarHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetAddress(string gemeentenaam, string straatnaam, string huisnummer,CancellationToken cancellationToken)
    {
        var requestQuery = new QueryString();
        requestQuery.Add("Gemeentenaam", gemeentenaam);
        requestQuery.Add("Straatnaam", straatnaam);
        requestQuery.Add("Huisnummer", huisnummer);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/adresmatch{requestQuery.Value}");

        var response = await _httpClient.SendAsync(request, cancellationToken);

        return response;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }


}
