namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using Acties.WijzigMaatschappelijkeZetel;
using FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Vereniging;

public class WijzigMaatschappelijkeZetelRequest
{
    public string? Naam { get; set; }
    public bool? IsPrimair { get; set; }

    public WijzigMaatschappelijkeZetelCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            locatieId, Naam, IsPrimair);
}
