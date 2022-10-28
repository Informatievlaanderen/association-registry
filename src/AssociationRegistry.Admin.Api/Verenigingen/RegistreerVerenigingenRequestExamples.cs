namespace AssociationRegistry.Admin.Api.Verenigingen;

using Swashbuckle.AspNetCore.Filters;

public class RegistreerVerenigingenRequestExamples : IExamplesProvider<RegistreerVerenigingRequest>
{
    public RegistreerVerenigingRequest GetExamples()
        => new() { Naam = "Naam van de vereniging" };
}
