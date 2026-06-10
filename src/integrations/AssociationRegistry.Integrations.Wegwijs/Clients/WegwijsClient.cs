namespace AssociationRegistry.Integrations.Wegwijs.Clients;

using System.Net.Http.Json;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using Framework;
using Responses;

public class WegwijsClient : IWegwijsClient
{
    private static readonly Guid WordtOpgevolgdDoorRelationId = new("2c68b8eb-55d2-ff8e-3301-f0fb12467df7");

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

            Throw<OrganisatieNietGevondenException>.If(
                organisationResponse is null || organisationResponse.Count == 0,
                ovoCode
            );

            return organisationResponse.First();
        }
        catch (OrganisatieNietGevondenException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new WegwijsException(e);
        }
    }

    public async Task<IReadOnlyList<string>> GetOpvolgerOrganisaties(
        string ovoCode,
        CancellationToken cancellationToken = default
    )
    {
        var opvolgers = new List<string>();
        string? volgendOvoCode = ovoCode;

        try
        {
            do
            {
                volgendOvoCode = await GetDirecteOpvolger(volgendOvoCode, cancellationToken);

                if (volgendOvoCode is not null)
                    opvolgers.Add(volgendOvoCode);
            } while (volgendOvoCode is not null);
        }
        catch (OrganisatieNietGevondenException)
        {
            return opvolgers;
        }

        return opvolgers;
    }

    private async Task<string?> GetDirecteOpvolger(string ovoCode, CancellationToken cancellationToken)
    {
        var organisatie = await GetOrganisationByOvoCode(ovoCode, cancellationToken);
        return HaalOpvolgerOvoCodeUitRelaties(organisatie);
    }

    private static string? HaalOpvolgerOvoCodeUitRelaties(OrganisationResponse organisatie) =>
        organisatie
            .Relations.FirstOrDefault(r => r.RelationId == WordtOpgevolgdDoorRelationId)
            ?.RelatedOrganisationOvoNumber;
}
