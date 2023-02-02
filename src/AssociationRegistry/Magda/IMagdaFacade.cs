namespace AssociationRegistry.Magda;

using INSZ;

public interface IMagdaFacade
{
    //Task<IEnumerable<Vertegenwoordiger>?> GetVertegenwoordigers(IEnumerable<RegistreerVerenigingCommand.Vertegenwoordiger>? vertegenwoordigers, CancellationToken token = default);
    Task<MagdaPersoon> GetByInsz(Insz insz, CancellationToken cancellationToken = default);
}
