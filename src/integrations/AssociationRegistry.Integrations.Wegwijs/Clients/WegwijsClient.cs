namespace AssociationRegistry.Integrations.Wegwijs.Clients;

using System.Net.Http.Json;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using Responses;

public class WegwijsClient : IWegwijsClient
{
    private readonly HttpClient _httpClient;

    public WegwijsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OrganisationResponse> GetOrganisationByOvoCode(
        string ovoCode,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await _httpClient.GetAsync($"search/organisations?q=ovoNumber:{ovoCode}", cancellationToken);

            response.EnsureSuccessStatusCode();

            var organisationResponse = await response.Content.ReadFromJsonAsync<List<OrganisationResponse>>(
                cancellationToken: cancellationToken
            );

            if (organisationResponse is null || organisationResponse.Count == 0)
            {
                throw new OrganisatieNietGevondenException(ovoCode);
            }

            return organisationResponse.First();
        }
        catch (OrganisatieNietGevondenException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new WegwijsException(e.InnerException);
        }
    }
}
