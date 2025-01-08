namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using Acties.Locaties.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Vereniging;

public class WijzigMaatschappelijkeZetelRequest
{
    public TeWijzigenMaatschappelijkeZetel Locatie { get; set; } = null!;

    public WijzigMaatschappelijkeZetelCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            TeWijzigenMaatschappelijkeZetel.Map(Locatie, locatieId));
}
