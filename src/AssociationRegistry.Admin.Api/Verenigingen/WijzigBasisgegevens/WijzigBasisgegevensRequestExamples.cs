namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using Swashbuckle.AspNetCore.Filters;

public class WijzigBasisgegevensRequestExamples : IExamplesProvider<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequest GetExamples()
        => new() { Naam = "Naam van de vereniging", KorteNaam = "Korte naam van de vereniging", Initiator = "OVO000001" };
}
