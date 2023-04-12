namespace AssociationRegistry.Vertegenwoordigers;

using Contactgegevens;
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

    public async Task<Vertegenwoordiger[]> GetVertegenwoordigersLijst(Vertegenwoordiger[]? vertegenwoordigers)
    {
        if (vertegenwoordigers is null) return Array.Empty<Vertegenwoordiger>();

        var expandedVertegenwoordigers = new List<Vertegenwoordiger>();

        foreach (var vert in vertegenwoordigers)
        {
            expandedVertegenwoordigers.Add(await GetVertegenwoordiger(vert));
        }

        return expandedVertegenwoordigers.ToArray();
    }

    private async Task<Vertegenwoordiger> GetVertegenwoordiger(Vertegenwoordiger vertegenwoordiger)
    {
        var insz = Insz.Create(vertegenwoordiger.Insz);
        var contactgegevens = Contactgegevens.FromArray(vertegenwoordiger.Contactgegevens.Select(c => Contactgegeven.Create(c.Type, c.Waarde, c.Omschrijving, c.IsPrimair)).ToArray());

        var magdaPersoon = await TryGetMagdaPersoon(insz);

        return Vertegenwoordiger.Create(
            insz,
            vertegenwoordiger.PrimairContactpersoon,
            vertegenwoordiger.Roepnaam,
            vertegenwoordiger.Rol,
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
