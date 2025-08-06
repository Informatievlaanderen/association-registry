namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;

using AssociationRegistry.Vereniging;
using Common;
using DecentraalBeheer.Acties.Locaties.VoegLocatieToe;
using DecentraalBeheer.Vereniging;
using Adres = DecentraalBeheer.Vereniging.Adressen.Adres;
using AdresId = DecentraalBeheer.Vereniging.Adressen.AdresId;

public class VoegLocatieToeRequest
{
    public ToeTeVoegenLocatie Locatie { get; set; } = null!;

    public VoegLocatieToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            DecentraalBeheer.Vereniging.Locatie.Create(
                Locatienaam.Create(Locatie.Naam ?? string.Empty),
                Locatie.IsPrimair,
                Locatie.Locatietype,
                Locatie.AdresId is not null
                    ? AdresId.Create(
                        Locatie.AdresId.Broncode,
                        Locatie.AdresId.Bronwaarde)
                    : null,
                Locatie.Adres is not null
                    ? Adres.Create(
                        Locatie.Adres.Straatnaam,
                        Locatie.Adres.Huisnummer,
                        Locatie.Adres.Busnummer,
                        Locatie.Adres.Postcode,
                        Locatie.Adres.Gemeente,
                        Locatie.Adres.Land)
                    : null));
}
