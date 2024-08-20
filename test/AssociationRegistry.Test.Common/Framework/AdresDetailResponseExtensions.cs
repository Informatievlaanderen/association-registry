namespace AssociationRegistry.Test.Common.Framework;

using Events;
using Grar.Models;

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
