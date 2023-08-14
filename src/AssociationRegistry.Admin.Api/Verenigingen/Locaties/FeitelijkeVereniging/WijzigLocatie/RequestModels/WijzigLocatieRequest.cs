﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;

using AssociationRegistry.Acties.WijzigLocatie;
using Vereniging;

public class WijzigLocatieRequest
{
    public TeWijzigenLocatie Locatie { get; set; } = null!;

    public WijzigLocatieCommand ToCommand(string vCode, int locatieId)
        => new(
            VCode.Create(vCode),
            TeWijzigenLocatie.Map(Locatie, locatieId));


}
