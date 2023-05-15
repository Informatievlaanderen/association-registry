namespace AssociationRegistry.Magda;

using Vereniging;

public interface IMagdaFacade
{
    //Task<IEnumerable<Vertegenwoordiger>?> GetVertegenwoordigers(IEnumerable<RegistreerFeitelijkeVerenigingCommand.Vertegenwoordiger>? vertegenwoordigers, CancellationToken token = default);
    Task<MagdaPersoon> GetByInsz(Insz insz, CancellationToken cancellationToken = default);
}
