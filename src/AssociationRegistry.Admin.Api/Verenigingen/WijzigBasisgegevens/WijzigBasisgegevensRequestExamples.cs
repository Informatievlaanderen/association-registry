namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using Swashbuckle.AspNetCore.Filters;
using WijzigBasisgegevens;

public class WijzigBasisgegevensRequestExamples : IExamplesProvider<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequest GetExamples()
        => new() { Naam = "Naam van de vereniging", Initiator = "OVO000001"};
}
