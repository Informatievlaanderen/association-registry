namespace AssociationRegistry.Events;

using Grar.Models;
using System.Text.RegularExpressions;

public record AdresDetailUitAdressenregister
{
    public static AdresDetailUitAdressenregister FromResponse(AddressDetailResponse response)
        => new()
        {
            AdresId = response.AdresId,
            Adres = new Registratiedata.AdresUitAdressenregister(
                response.Straatnaam,
                response.Huisnummer,
                response.Busnummer,
                response.Postcode,
                response.Gemeente),
        };

    public Registratiedata.AdresId AdresId { get; init; }
    public Registratiedata.AdresUitAdressenregister Adres { get; init; }
}
