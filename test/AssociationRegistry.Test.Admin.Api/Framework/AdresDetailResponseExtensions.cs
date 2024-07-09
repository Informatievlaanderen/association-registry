namespace AssociationRegistry.Test.Admin.Api.Framework;

using AssociationRegistry.Grar.Models;
using Events;

public static class AdresDetailResponseExtensions
{
    public static Registratiedata.AdresUitAdressenregister ToAdresUitAdressenregister(this AddressDetailResponse response)
        => new Registratiedata.AdresUitAdressenregister(
            response.Straatnaam,
            response.Huisnummer,
            response.Busnummer,
            response.Postcode,
            response.Gemeente);
}
