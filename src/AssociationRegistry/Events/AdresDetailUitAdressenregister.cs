namespace AssociationRegistry.Events;

using Grar.Models;

public record AdresDetailUitAdressenregister
{
    public static AdresDetailUitAdressenregister FromResponse(AddressDetailResponse response)
        => new()
        {
            AdresId = response.AdresId,
            Adres = new Registratiedata.Adres(
                response.Straatnaam,
                response.Huisnummer,
                response.Busnummer,
                response.Postcode,
                response.Gemeente,
                "België")
        };

    public Registratiedata.AdresId AdresId { get; init; }
    public Registratiedata.Adres Adres { get; init; }
}
