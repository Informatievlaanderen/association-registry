namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;

using AssociationRegistry.DecentraalBeheer.Locaties.VoegLocatieToe;
using AssociationRegistry.Vereniging;
using Common;
using Adres = Vereniging.Adres;
using AdresId = Vereniging.AdresId;

public class VoegLocatieToeRequest
{
    public ToeTeVoegenLocatie Locatie { get; set; } = null!;

    public VoegLocatieToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            AssociationRegistry.Vereniging.Locatie.Create(
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
