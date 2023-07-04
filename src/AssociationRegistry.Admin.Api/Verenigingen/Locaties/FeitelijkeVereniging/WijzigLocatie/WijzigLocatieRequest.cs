namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;

using Acties.WijzigLocatie;
using Acties.WijzigVertegenwoordiger;
using Common;
using Vereniging;
using AdresId = Vereniging.AdresId;

public class WijzigLocatieRequest
{
    public ToeTeVoegenLocatie Locatie { get; set; } = null!;

    public WijzigLocatieCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            new WijzigLocatieCommand.CommandLocatie(
                locatieId,
                Locatie.Locatietype,
                Locatie.IsPrimair,
                Locatie.Naam,
                Locatie.Adres is not null
                    ? Adres.Create(
                        Locatie.Adres.Straatnaam,
                        Locatie.Adres.Huisnummer,
                        Locatie.Adres.Busnummer,
                        Locatie.Adres.Postcode,
                        Locatie.Adres.Gemeente,
                        Locatie.Adres.Land)
                    : null,
                Locatie.AdresId is not null
                    ? AdresId.Create(
                        Locatie.AdresId.Broncode,
                        Locatie.AdresId.Bronwaarde)
                    : null));
}
