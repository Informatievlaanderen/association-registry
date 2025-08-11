namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;

using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Locaties.WijzigLocatie;
using DecentraalBeheer.Vereniging;

public class WijzigLocatieRequest
{
    public TeWijzigenLocatie Locatie { get; set; } = null!;

    public WijzigLocatieCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            TeWijzigenLocatie.Map(Locatie, locatieId));
}
