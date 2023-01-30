namespace AssociationRegistry.Magda;

using Vereniging.RegistreerVereniging;
using Vertegenwoordigers;

public interface IMagdaFacade
{
    Task<VertegenwoordigersLijst> GetVertegenwoordigers(IEnumerable<RegistreerVerenigingCommand.Vertegenwoordiger> vertegenwoordigers, CancellationToken token = default);
}

public class MagdaPersoon
{
    public string Insz { get; set; } = null!;
    public string Voornaam { get; set; } = null!;
    public string Achternaam { get; set; } = null!;
    public bool IsOverleden { get; set; } = false;
}
