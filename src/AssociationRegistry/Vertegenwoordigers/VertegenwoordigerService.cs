namespace AssociationRegistry.Vertegenwoordigers;

using ContactInfo;
using Exceptions;
using INSZ;
using Magda;
using Magda.Exceptions;
using Vereniging;
using Vereniging.RegistreerVereniging;

public class VertegenwoordigerService
{
    private readonly IMagdaFacade _magdaFacade;

    public VertegenwoordigerService(IMagdaFacade magdaFacade)
    {
        _magdaFacade = magdaFacade;
    }

    public async Task<VertegenwoordigersLijst> GetVertegenwoordigersLijst(IEnumerable<RegistreerVerenigingCommand.Vertegenwoordiger>? vertegenwoordigers)
    {
        if (vertegenwoordigers is null) return VertegenwoordigersLijst.Empty;

        var expandedVertegenwoordigers = new List<Vertegenwoordiger>();

        foreach (var vert in vertegenwoordigers)
        {
            expandedVertegenwoordigers.Add(await GetVertegenwoordiger(vert));
        }

        return VertegenwoordigersLijst.Create(expandedVertegenwoordigers);
    }

    private async Task<Vertegenwoordiger> GetVertegenwoordiger(RegistreerVerenigingCommand.Vertegenwoordiger vertegenwoordiger)
    {
        var insz = Insz.Create(vertegenwoordiger.Insz);
        var contactLijst = ContactLijst.Create(vertegenwoordiger.ContactInfoLijst);

        var magdaPersoon = await TryGetMagdaPersoon(insz);

        return Vertegenwoordiger.Create(insz, vertegenwoordiger.PrimairContactpersoon, vertegenwoordiger.Roepnaam, vertegenwoordiger.Rol, magdaPersoon.Voornaam, magdaPersoon.Achternaam, contactLijst);
    }

    private async Task<MagdaPersoon> TryGetMagdaPersoon(Insz insz)
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
