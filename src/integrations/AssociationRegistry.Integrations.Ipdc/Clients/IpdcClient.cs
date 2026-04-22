namespace AssociationRegistry.Integrations.Ipdc.Clients;

using System.Net.Http.Json;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Ipdc;
using Framework;
using Responses;

public class IpdcClient : IIpdcClient
{
    public readonly HttpClient HttpClient;

    public IpdcClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public async Task<IpdcProductResponse?> GetById(
        string ipdcProductNummer,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await HttpClient.GetAsync($"id/concept/{ipdcProductNummer}", cancellationToken);

            Throw<OngeldigIpdcProductNummer>.If(
                response.StatusCode == System.Net.HttpStatusCode.BadRequest,
                ipdcProductNummer
            );
            Throw<OnbekendIpdcProductNummer>.If(
                response.StatusCode == System.Net.HttpStatusCode.NotFound,
                ipdcProductNummer
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IpdcProductResponse>(cancellationToken: cancellationToken);
        }
        catch (OngeldigIpdcProductNummer)
        {
            throw;
        }
        catch (OnbekendIpdcProductNummer)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new IpdcException(e);
        }
    }
}
