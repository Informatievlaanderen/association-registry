namespace AssociationRegistry.Vertegenwoordigers;

using ContactInfo;
using Exceptions;
using INSZ;
using Magda;
using Magda.Exceptions;
using Vereniging.RegistreerVereniging;

public class VertegenwoordigerService
{
    private readonly IMagdaFacade _magdaFacade;

    public VertegenwoordigerService(IMagdaFacade magdaFacade)
    {
        _magdaFacade = magdaFacade;
    }

    public async Task<Vertegenwoordiger> CreateVertegenwoordiger(
        Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        ContactLijst contactLijst)
    {
        var magdaPersoon = await TryGetByInsz(insz);
        return Vertegenwoordiger.Create(insz, primairContactpersoon, roepnaam, rol, magdaPersoon.Voornaam, magdaPersoon.Achternaam, contactLijst);
    }

    private async Task<MagdaPersoon> TryGetByInsz(Insz insz)
    {
        try
        {
            return await _magdaFacade.GetByInsz(insz);
        }
        catch (MagdaException)
        {
            throw new UnknownInsz();
        }
    }
}
