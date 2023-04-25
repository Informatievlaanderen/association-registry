namespace AssociationRegistry.Vereniging;

using Exceptions;
using Magda;
using Magda.Exceptions;

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

        foreach (var vert in vertegenwoordigers) expandedVertegenwoordigers.Add(await GetVertegenwoordiger(vert));

        return expandedVertegenwoordigers.ToArray();
    }

    private async Task<Vertegenwoordiger> GetVertegenwoordiger(Vertegenwoordiger vertegenwoordiger)
    {
        var insz = Insz.Create(vertegenwoordiger.Insz);

        var magdaPersoon = await TryGetMagdaPersoon(insz);

        return Vertegenwoordiger.Enrich(
            vertegenwoordiger,
            magdaPersoon);
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
