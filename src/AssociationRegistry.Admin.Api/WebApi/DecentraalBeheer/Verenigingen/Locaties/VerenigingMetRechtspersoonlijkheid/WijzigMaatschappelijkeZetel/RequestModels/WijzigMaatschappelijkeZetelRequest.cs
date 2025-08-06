namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using AssociationRegistry.DecentraalBeheer.Locaties.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Vereniging;

public class WijzigMaatschappelijkeZetelRequest
{
    public TeWijzigenMaatschappelijkeZetel Locatie { get; set; } = null!;

    public WijzigMaatschappelijkeZetelCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            TeWijzigenMaatschappelijkeZetel.Map(Locatie, locatieId));
}
