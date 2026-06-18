namespace AssociationRegistry.Integrations.Wegwijs.Services;

using AssociationRegistry.Wegwijs;
using Clients;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using Responses;

public class OrganisatieBevoegdheidService : IOrganisatieBevoegdheidService
{
    public static readonly Guid WordtOpgevolgdDoorRelationId = new("2c68b8eb-55d2-ff8e-3301-f0fb12467df7");
    private readonly IWegwijsClient _client;

    public OrganisatieBevoegdheidService(IWegwijsClient client)
    {
        _client = client;
    }

    public async Task<string[]> GetOpvolgers(string initiator)
    {
        var opvolgers = new List<string>();
        string? volgendOvoCode = initiator;

        try
        {
            do
            {
                volgendOvoCode = await GetDirecteOpvolger(volgendOvoCode, CancellationToken.None);

                if (initiator == volgendOvoCode || opvolgers.Contains(volgendOvoCode))
                {
                    return opvolgers.ToArray();
                }

                if (volgendOvoCode is not null)
                    opvolgers.Add(volgendOvoCode);
            } while (volgendOvoCode is not null);
        }
        catch (OrganisatieNietGevondenException) { }

        return opvolgers.ToArray();
    }

    private async Task<string?> GetDirecteOpvolger(string ovoCode, CancellationToken cancellationToken)
    {
        var organisatie = await _client.GetOrganisationByOvoCode(ovoCode, cancellationToken);

        return HaalOpvolgerOvoCodeUitRelaties(organisatie);
    }

    private static string? HaalOpvolgerOvoCodeUitRelaties(OrganisationResponse organisatie) =>
        organisatie
           .Relations.FirstOrDefault(r => r.RelationId == WordtOpgevolgdDoorRelationId)
          ?.RelatedOrganisationOvoNumber;
}
