namespace AssociationRegistry.Vertegenwoordigers;

using ContactGegevens;
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

    public async Task<VertegenwoordigersLijst> GetVertegenwoordigersLijst(IEnumerable<RegistreerVerenigingCommand.TeRegistrerenVertegenwoordiger>? vertegenwoordigers)
    {
        if (vertegenwoordigers is null) return VertegenwoordigersLijst.Empty;

        var expandedVertegenwoordigers = new List<Vertegenwoordiger>();

        foreach (var vert in vertegenwoordigers)
        {
            expandedVertegenwoordigers.Add(await GetVertegenwoordiger(vert));
        }

        return VertegenwoordigersLijst.Create(expandedVertegenwoordigers);
    }

    private async Task<Vertegenwoordiger> GetVertegenwoordiger(RegistreerVerenigingCommand.TeRegistrerenVertegenwoordiger teRegistrerenVertegenwoordiger)
    {
        var insz = Insz.Create(teRegistrerenVertegenwoordiger.Insz);
        var contactgegevens = Contactgegevens.FromArray(teRegistrerenVertegenwoordiger.Contactgegevens.Select(c => Contactgegeven.Create(c.Type, c.Waarde, c.Omschrijving, c.IsPrimair)).ToArray());

        var magdaPersoon = await TryGetMagdaPersoon(insz);

        return Vertegenwoordiger.Create(
            insz,
            teRegistrerenVertegenwoordiger.PrimairContactpersoon,
            teRegistrerenVertegenwoordiger.Roepnaam,
            teRegistrerenVertegenwoordiger.Rol,
            magdaPersoon.Voornaam,
            magdaPersoon.Achternaam,
            contactgegevens);
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
