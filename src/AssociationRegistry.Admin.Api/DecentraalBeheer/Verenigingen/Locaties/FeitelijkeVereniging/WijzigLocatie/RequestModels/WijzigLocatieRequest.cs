namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;

using AssociationRegistry.DecentraalBeheer.Locaties.WijzigLocatie;
using AssociationRegistry.Vereniging;

public class WijzigLocatieRequest
{
    public TeWijzigenLocatie Locatie { get; set; } = null!;

    public WijzigLocatieCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            TeWijzigenLocatie.Map(Locatie, locatieId));
}
