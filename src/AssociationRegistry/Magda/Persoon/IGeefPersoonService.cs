namespace AssociationRegistry.Magda.Persoon;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;

public interface IGeefPersoonService
{
    Task<PersonenUitKsz> GeefPersonen(Vertegenwoordiger[] vertegenwoordigers, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<PersoonUitKsz> GeefPersoon(Vertegenwoordiger vertegenwoordiger, CommandMetadata metadata, CancellationToken cancellationToken);
}
