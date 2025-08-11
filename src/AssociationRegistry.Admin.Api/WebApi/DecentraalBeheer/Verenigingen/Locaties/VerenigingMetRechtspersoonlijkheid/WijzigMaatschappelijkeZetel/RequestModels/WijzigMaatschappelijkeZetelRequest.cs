namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Locaties.WijzigMaatschappelijkeZetel;
using DecentraalBeheer.Vereniging;

public class WijzigMaatschappelijkeZetelRequest
{
    public TeWijzigenMaatschappelijkeZetel Locatie { get; set; } = null!;

    public WijzigMaatschappelijkeZetelCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            TeWijzigenMaatschappelijkeZetel.Map(Locatie, locatieId));
}
