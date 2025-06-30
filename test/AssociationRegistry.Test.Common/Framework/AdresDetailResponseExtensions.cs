namespace AssociationRegistry.Test.Common.Framework;

using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Grar.Models;

public static class AdresDetailResponseExtensions
{
    public static Registratiedata.AdresUitAdressenregister ToAdresUitAdressenregister(this AddressDetailResponse response)
        => new(
            response.Straatnaam,
            response.Huisnummer,
            response.Busnummer,
            response.Postcode,
            response.Gemeente);
}
