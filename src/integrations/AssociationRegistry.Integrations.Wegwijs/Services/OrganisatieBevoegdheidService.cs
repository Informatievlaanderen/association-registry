namespace AssociationRegistry.Integrations.Wegwijs.Services;

using AssociationRegistry.Wegwijs;
using Clients;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using Framework;
using Responses;

public class OrganisatieBevoegdheidService : IOrganisatieBevoegdheidService
{
    public static readonly Guid WordtOpgevolgdDoorRelationId = new("2c68b8eb-55d2-ff8e-3301-f0fb12467df7");

    private readonly IWegwijsClient _client;

    public OrganisatieBevoegdheidService(IWegwijsClient client)
    {
        _client = client;
    }

    public async Task<string[]> GetGemachtigdeOrganisaties(string initiator, string geregistreerdDoor)
    {
        if (initiator == geregistreerdDoor)
            return [];

        var opvolgers = await GetOpvolgers(geregistreerdDoor);

        Throw<GiIsNietBevoegd>.If(!opvolgers.Contains(initiator));

        return opvolgers.ToArray();
    }

    private async Task<List<string>> GetOpvolgers(string geregistreerdDoor)
    {
        var opvolgers = new List<string>();
        string? volgendOvoCode = geregistreerdDoor;

        try
        {
            do
            {
                volgendOvoCode = await GetDirecteOpvolger(volgendOvoCode, CancellationToken.None);

                if (volgendOvoCode is not null)
                    opvolgers.Add(volgendOvoCode);
            } while (volgendOvoCode is not null);
        }
        catch (OrganisatieNietGevondenException)
        {
        }

        return opvolgers;
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
