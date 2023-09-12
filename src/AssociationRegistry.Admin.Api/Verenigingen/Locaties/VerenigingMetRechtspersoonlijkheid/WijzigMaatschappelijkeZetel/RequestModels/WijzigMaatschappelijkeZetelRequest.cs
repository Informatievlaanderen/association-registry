namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using Acties.WijzigMaatschappelijkeZetel;
using Vereniging;

public class WijzigMaatschappelijkeZetelRequest
{
    public TeWijzigenMaatschappelijkeZetel Locatie { get; set; } = null!;

    public WijzigMaatschappelijkeZetelCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            TeWijzigenMaatschappelijkeZetel.Map(Locatie, locatieId));
}
