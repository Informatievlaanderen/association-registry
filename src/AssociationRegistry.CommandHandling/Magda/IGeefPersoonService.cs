namespace AssociationRegistry.CommandHandling.Magda;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using DecentraalBeheer.Middleware;
using Framework;

public interface IGeefPersoonService
{
    Task<PersonenUitKsz> GeefPersonen(Vertegenwoordiger[] vertegenwoordigers, CommandMetadata metadata, CancellationToken cancellationToken);
}
