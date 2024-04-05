namespace AssociationRegistry.Grar;

using Microsoft.AspNetCore.Http;
using Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
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
        var response = await _httpClient.GetAsync($"/v2/adresmatch?Gemeentenaam={gemeentenaam}&Straatnaam={straatnaam}&Huisnummer={huisnummer}", cancellationToken);

        var str = await response.Content.ReadAsStringAsync();

        var json = JsonConvert.DeserializeObject<AddressMatchOsloCollection>(str);

        var responsejson = await response.Content.ReadFromJsonAsync<AddressMatchOsloCollection>();

        return response;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }


}
