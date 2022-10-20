namespace AssociationRegistry.Admin.Api.Verenigingen;

using Swashbuckle.AspNetCore.Filters;

public class CreateVerenigingenRequestExamples : IExamplesProvider<CreateVerenigingCommand>
{
    public CreateVerenigingCommand GetExamples()
        => new("Naam van de vereniging");
}
