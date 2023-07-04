namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;

using AssociationRegistry.Acties.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Vereniging;
using AdresId = Vereniging.AdresId;

public class WijzigLocatieRequest
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
