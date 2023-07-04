namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;

using Acties.VoegLocatieToe;
using Common;
using Vereniging;
using AdresId = Vereniging.AdresId;
using Adres = Vereniging.Adres;

public class VoegLocatieToeRequest
{
    public ToeTeVoegenLocatie Locatie { get; set; } = null!;

    public VoegLocatieToeCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            AssociationRegistry.Vereniging.Locatie.Create(
                Locatie.Naam,
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
