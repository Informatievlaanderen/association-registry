namespace AssociationRegistry.Grar.AdresMatch;

using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Adressen.GemeentenaamVerrijking;
using Grar.Models;

public record VerrijktAdresUitGrar
{
    public IAddressResponse AddressResponse { get; }
    public VerrijkteGemeentenaam Gemeente { get; }
    public Adres OrigineleAdres { get; }

    public VerrijktAdresUitGrar(IAddressResponse addressResponse, VerrijkteGemeentenaam gemeente, Adres origineleAdres)
    {
        AddressResponse = addressResponse;
        Gemeente = gemeente;
        OrigineleAdres = origineleAdres;
    }

    public Adres ToAdres()
        => Adres.Hydrate(
            straatnaam: AddressResponse.Straatnaam,
            huisnummer: AddressResponse.Huisnummer,
            busnummer: AddressResponse.Busnummer,
            postcode: AddressResponse.Postcode,
            gemeente: Gemeente.Naam,
            land: Adres.België);
}
