namespace AssociationRegistry.Integrations.Wegwijs.Services;

using AssociationRegistry.Wegwijs;
using Clients;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Framework;

public class OrganisatieBevoegdheidService : IOrganisatieBevoegdheidService
{
    private readonly IWegwijsClient _client;

    public OrganisatieBevoegdheidService(IWegwijsClient client)
    {
        _client = client;
    }

    public async Task<string[]> IsGemachtigdeOrganisatie(string initiator, string geregistreerdDoor)
    {
        if (initiator == geregistreerdDoor)
            return [];

        var opvolgers = await _client.GetOpvolgerOrganisaties(geregistreerdDoor);
        Throw<GiIsNietBevoegd>.If(!opvolgers.Contains(initiator));

        return opvolgers.ToArray();
    }
}
