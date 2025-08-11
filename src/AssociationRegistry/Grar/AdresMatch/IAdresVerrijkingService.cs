namespace AssociationRegistry.Grar.AdresMatch;

using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Grar.Models;

public interface IAdresVerrijkingService
{
    Task<Registratiedata.AdresUitAdressenregister> VerrijkAdres(
        AddressMatchResponse matchResponse,
        Adres origineleAdres,
        CancellationToken cancellationToken);
}
