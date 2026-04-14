namespace AssociationRegistry.Integrations.Ipdc.Clients;

using System.Net.Http.Json;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Responses;

public class IpdcClient : IIpdcClient
{
    private readonly HttpClient _httpClient;

    public IpdcClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IpdcProductResponse?> GetById(
        string ipdcProductNummer,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient.GetAsync($"products/{ipdcProductNummer}", cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new OnbekendIpdcProductNummer(ipdcProductNummer);
        }

        response.EnsureSuccessStatusCode();

        var ipdcProductResponse = await response.Content.ReadFromJsonAsync<IpdcProductResponse>(
            cancellationToken: cancellationToken
        );

        if (ipdcProductResponse == null)
        {
            throw new OnbekendIpdcProductNummer(ipdcProductNummer);
        }

        return ipdcProductResponse;
    }
}
