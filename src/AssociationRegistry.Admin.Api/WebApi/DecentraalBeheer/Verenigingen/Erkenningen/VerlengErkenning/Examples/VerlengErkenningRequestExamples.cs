namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.VerlengErkenning.Examples;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class VerlengErkenningRequestExamples : IExamplesProvider<SchorsErkenningRequest>
{
    public SchorsErkenningRequest GetExamples() =>
        new()
        {
            RedenSchorsing = "Niet-naleving van de erkenningsvoorwaarden zoals bepaald in het reglement.",
        };
}
