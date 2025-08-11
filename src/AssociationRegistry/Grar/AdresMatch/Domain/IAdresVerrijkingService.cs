namespace AssociationRegistry.Grar.AdresMatch.Domain;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Vereniging;

public interface IAdresVerrijkingService
{
    Task<Registratiedata.AdresUitAdressenregister> VerrijkAdres(
        AddressMatchResponse matchResponse,
        Adres origineleAdres,
        CancellationToken cancellationToken);
}
