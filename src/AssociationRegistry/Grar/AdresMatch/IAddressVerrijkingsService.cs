namespace AssociationRegistry.Grar.AdresMatch;

using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Grar.Models;
using GemeentenaamVerrijking;

public interface IAddressVerrijkingsService
{
    Task<VerrijktAdresUitGrar> FromAdresAndGrarResponse(
        IAddressResponse matchResponse,
        Adres origineleAdres,
        CancellationToken cancellationToken);

    Task<VerrijktAdresUitGrar> FromActiefAdresId(
        AdresId adresId,
        CancellationToken cancellationToken);
}
