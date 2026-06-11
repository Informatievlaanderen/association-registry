namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class CorrigeerSchorsingErkenningRequestExamples : IExamplesProvider<CorrigeerRedenSchorsingErkenningRequest>
{
    public CorrigeerRedenSchorsingErkenningRequest GetExamples() =>
        new() { RedenSchorsing = "Niet-naleving van de erkenningsvoorwaarden zoals bepaald in het reglement." };
}
