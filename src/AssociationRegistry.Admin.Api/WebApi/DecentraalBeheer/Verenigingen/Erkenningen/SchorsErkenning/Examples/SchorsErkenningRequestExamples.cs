namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class SchorsErkenningRequestExamples : IExamplesProvider<SchorsErkenningRequest>
{
    public SchorsErkenningRequest GetExamples() =>
        new()
        {
            RedenSchorsing = "Niet-naleving van de erkenningsvoorwaarden zoals bepaald in het reglement.",
        };
}
